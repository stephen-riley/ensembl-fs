# EnsemblFS

A simple virtual filesystem to navigate Ensembl genomic data using FUSE

## More to follow...

The basic idea is you should be able to do the following:

```bash
~> cd /tmp/ensembl/homo_sapiens/v99/chromosomes/1
~> cat REF
GCCATCGCTGCCACAGAACCCAGTGGATTGGCCTAGGTGGGATCTCTGAGCTCAACAAGC
CCTCTCTGGGTGGTAGGTGCAGAGACGGGAGGGGCAGAGCCGCAGGCACAGCCAAGAGGG
CTGAAGAAATGGTAGAACGGAGCAGCTGGTGATGTGTGGGCCCACCGGCCCCAGGCTCCT
GTCTCCCCCCAGGTGTGTGGTGATGCCAGGCATGCCCTTCCCCAGCATCAGGTCTCCAGA
GCTGCAGAAGACGACGGCCGACTTGGATCACACTCTTGTGAGTGTCCCCAGTGTTGCAGA
...
```

and the data will be pulled live from the EnsEMBL genomic databases located around the world.
