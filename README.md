# EnsemblFS

A simple virtual filesystem to navigate Ensembl genomic data using FUSE

## Conceptual demo

The basic idea is you should be able to do the following:

```bash
~> cd /tmp/ensembl/homo_sapiens_core_99_38/chromosomes/1
~> cat REF
GCCATCGCTGCCACAGAACCCAGTGGATTGGCCTAGGTGGGATCTCTGAGCTCAACAAGC
CCTCTCTGGGTGGTAGGTGCAGAGACGGGAGGGGCAGAGCCGCAGGCACAGCCAAGAGGG
CTGAAGAAATGGTAGAACGGAGCAGCTGGTGATGTGTGGGCCCACCGGCCCCAGGCTCCT
GTCTCCCCCCAGGTGTGTGGTGATGCCAGGCATGCCCTTCCCCAGCATCAGGTCTCCAGA
GCTGCAGAAGACGACGGCCGACTTGGATCACACTCTTGTGAGTGTCCCCAGTGTTGCAGA
...
```

...and the data will be pulled live from the EnsEMBL genomic databases located around the world.

```text
/ensemblfs
    /homo_sapiens_core_99_38    # this is the ensembl species database
        /features
        /proteiens
        /chromosomes
            /1
                REF             # the reference genomic data
                PATCHED         # ref, but with patches applied (PATCH, PATCH_FIX, etc.)
            /2
            /3
             :
```

See [HOWITWORKS](https://github.com/stephen-riley/ensembl-fs/blob/master/HOWITWORKS.md) for a description of how EnsemblFS works with Ensembl genomic databases.
