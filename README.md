# TransactionProcessorACL

## HTTPS listener

The service listens on HTTPS port `5003` only. The `TransactionProcessing.CertificateDefaults`
shared library configures Kestrel to load the CA-issued server certificate from these environment variables:

- `ServiceOptions__KestrelOptions__Path` — path to the mounted `.pfx` file, or full-chain PEM file.
- `ServiceOptions__KestrelOptions__Password` — PFX password, supplied as a deployment secret.
- `ServiceOptions__KestrelOptions__KeyPath` — private key path when using PEM.

The certificate must include the service hostname in its subject alternative names, and
clients must trust its issuing CA. Mount certificate files read-only and restrict access
to the service identity. Do not commit certificates, private keys, or passwords. The port
number remains unchanged.

## Build Status

[![Last Nightly Build Status Badge](https://github.com/TransactionProcessing/TransactionProcessorACL/workflows/Nightly%20Build/badge.svg)](https://github.com/TransactionProcessing/TransactionProcessorACL/workflows/Nightly%20Build/badge.svg)
[![Last Release Build Status Badge](https://github.com/TransactionProcessing/TransactionProcessorACL/workflows/Release/badge.svg)](https://github.com/TransactionProcessing/TransactionProcessorACL/workflows/Release/badge.svg)


## Nuget Versions
|||
| --- | --- |
| **TransactionProcessorACL.DataTransferObjects** | [![TransactionProcessorACL DataTransferObjects MyGet Badge](https://buildstats.info/myget/transactionprocessing/TransactionProcessorACL.DataTransferObjects)](https://buildstats.info/myget/transactionprocessing/TransactionProcessorACL.DataTransferObjects) |

## Code Coverage Results

[![Coverage Status](https://coveralls.io/repos/github/StuartFerguson/TransactionProcessorACL/badge.svg)](https://coveralls.io/github/StuartFerguson/TransactionProcessorACL)

## Code Quality Results

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/0c614670e9e34ddba1d04c0d790fc969)](https://www.codacy.com/manual/stuart_ferguson1/TransactionProcessorACL?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=TransactionProcessing/TransactionProcessorACL&amp;utm_campaign=Badge_Grade)

![Alt](https://repobeats.axiom.co/api/embed/8dba624b603ade6802426c5a037174aff11e1f2d.svg "Repobeats analytics image")
