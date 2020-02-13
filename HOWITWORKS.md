# How EnsemblFS works

Briefly, EnsemblFS is a [FUSE-based](https://osxfuse.github.io/) virtual file system for macOS that queries an [Ensembl database](https://uswest.ensembl.org/info/docs/api/core/core_schema.html) to assemble the genetic data for a chromosome.

For the purposes of this document, we will only be looking at [DNA base pair](https://en.wikipedia.org/wiki/Nucleobase) data.

## The Ensembl database

An Ensembl database for a given species contains a tremendous amount of genetic information.  There are some very important concepts in the database to understand how it works:

1. Coordinate systems
2. Sequence regions
3. Assemblies
4. How to handle gaps

### Coordinate systems

Take a look at the `coord_system` table and you'll see these fields (in addition to others):

* `version`: The version identifier of the the data.  For humans, we're looking for `GRCh38`.
* `name`: To get the top level list of chromosomes, we are looking for the name `chromosome` for the appropriate `version`.  To get to actual DNA data, we want `contig` (though see `attrib` for the real reason).
* `attrib`: In general, we only want to use the coordinate systems with attribute `default_version`.  Also, to get the appropriate coordinate system for DNA data, we want to find the `sequence_level` attribute.

In the case of the current homo sapiens database, the two coordinate systems we're interested in are `4`, for chromosomes, and `1`, for DNA.

### Sequence regions

Sequence regions are the primary index of named *genetic things* in the database.  (I'm not up on genetics research enough to know a better term than "things").  They are stored in the `seq_region` table.

This is how you can find the list of chromosomes, for example.

```sql
select * from seq_region
where coord_system_id=4;
```

(Remember, coordinate system `4` is the `GRCh38 chromosome` coordinate system.)

From this query, you'll get hundreds of items, 25 of which are the well known chromosomes for humans: `1` through `22`, `X`, `Y`, and the mitochondrial sequence dubbed `MT`.

By noting the `seq_region_id` for one of these chromosomes (eg. chromosome `1` has sequence region ID `131550`), you are one step further toward find the actual DNA sequences.  This is where *assemblies* come in.

### Assemblies

[*Genome assemblies*](https://uswest.ensembl.org/info/genome/genebuild/assembly.html) are the construct by which little sequences of DNA are linked together (database-wise) into larger objects, like chromosomes.

Ensembl assemblies are stored in the `assembly` table.  It builds a parent-children relationship among the sequence regions:

* `asm_seq_region_id` (*assembly sequence region ID*) is the parent sequence region.  For example, sequence region `131550` (chromosome `1`) is the parent when you're trying to find the sequences of DNA (the children).
* `cmp_seq_region_id` (*component sequence region ID*) is the child sequence region.
* `asm_start` and `asm_end` mark the positions *in the parent* that the component sequence region occupy.  Note that these numbers are 1-based, not 0-based.  (That is, the first element in an assembly is at position 1).
* `cmp_start` and `cmp_end` mark the positions *in the child sequence* that are part of the parent.  Again, these are 1-based.
* `ori` is the orientation of the component sequence.  This can be `1` for [sense](https://en.wikipedia.org/wiki/Sense_strand) sequences (5' to 3' orientation), or `-1` for [antisense](https://en.wikipedia.org/wiki/Sense_(molecular_biology)) sequences (3' to 5' orientation, complemented).

However, if you run this query to get all the components of the `1` chromosome, you won't get what you want.

```sql
select * from assembly
where asm_seq_region_id=131550
order by asm_start;
```

Look carefully in this list and you'll see two important things:

1. The assembly doesn't start at 1!  In fact, it appears to start at 10,001.  We'll come back to this when we talk about how to handle gaps.
2. We see several entries that all start in the same assembly location!  How can this be?

The answer to #2 is that there is a mix of coordinate systems present in the query results.  We need to filter this so we only get `sequence_level` results back (which in our previous example is coordinate system `1`).

```sql
select a.* from assembly a, seq_region r
where r.coord_system_id=1
and a.asm_seq_region_id=131550
and r.seq_region_id=a.cmp_seq_region_id
order by asm_start;
```

Now we have all the child sequence regions for chromosome `1` in order, with clear visibility on the gaps.

### How to handle gaps

In the Ensembl database, there are often gaps between components in an assembly.  When this happens, we can just insert `N` characters in place of base pair codes (`ATCG`).  This denotes an "unknown" region in the sequence.

## Extracting DNA from the database

Now that we understand how all the associations work in the database, we have only to extract the DNA.  DNA is stored in the `dna` table, which contains strings of DNA codes for a given sequence region.

```sql
select sequence from dna where seq_region_id in (
    select a.cmp_seq_region_id from assembly a, seq_region r
    where r.coord_system_id=1
    and a.asm_seq_region_id=131550
    and r.seq_region_id=a.cmp_seq_region_id
    order by asm_start
);
```

Now you have the DNA for each component.  However, these strings need to be trimmed according to the `cmp_start` and `cmp_end` fields of the `assembly` table.  This is a simple matter of extracting a substring for components that are *sense* strands (`ori=1`).  *Antisense* strands are a different story.

## Dealing with antisense sequences

Antisense sequences are complementary sequences, meaning they need to be looked at backwards and their base pairs complemented.

This is easier to understand with an example.  Let's say you are looking at an antisense sequence, with `cmp_start`=2 and `cmp_end`=4.  Let's say the `dna.sequence` field contains this:

```text
dna.sequence
------------
AATTCCGGCC
```

Instead of taking the second through fourth base pairs (`ATTC`), you'd want to first reverse the sequence, complement it, and *then* take the second through fourth pairs.  So:

1. Reverse the sequence: `CCGGCCTTAA`
2. Complement the base pairs (swap `T` and `A`, swap `C` and `G`): `GGCCGGAATT`
3. Now take the second through fourth base pairs: `GCCG`

Of course, doing the reversal and complement on many large sequences is computationally expensive, so instead we just do a quick coordinate transform, extract only the relevant parts, and then do a reverse and complement.  See [`Slice.GetSequenceStrings()`](https://github.com/stephen-riley/ensembl-net/blob/55835cbb08999217518005dcd6ef70464b81db06/Ensembl/Slice.cs#L51) in the [Ensembl.NET](https://github.com/stephen-riley/ensembl-net) project for specifics.

## tl;dr

The EnsemblFS file system simply navigates the Ensembl database structure for you to assemble genetic data by mapping *sequence regions* of genetic data with specific *coordinate systems* through the *assembly* parent-child relationship table.
