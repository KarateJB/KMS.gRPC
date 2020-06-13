# KMS.gRPC

Key management System in ASP.NET Core 3 gRPC.

## Features

- Supports TripleDES, Secret(MD5), RSA keypair distribution.
- Custom cycle time to audit keys in client side.
- Custom cycle time to renew expired keys.
- Crypto library that supports
  * TripleDES encryption/decryption.
  * RSA encryption/decryption, sign/verify signaure.
- Use Redis to store the working or deprecated keys.

## Key exchange process

1. First get a session key (TripleDES key) from KMS.
2. Use the session key to encrypt the new keys (Secrets, RSA ...) from server to client, or auditing keys from client to server.


## About codes

| Project name | Project type | Description | Note |
|:-------------|:------------:|:------------|:-----|
| Kms.gRPC | ASP.NET Core gRPC Service | KMS (server side) | gRPC services |
| Kms.gRPC.Client | ASP.NET Core Web App | KMS client side | gRPC client |
| Kms.KeyMngr | Class library |  Key manangement library | |
| Kms.Client.Dispatcher | Class library | KMS client key exchange/renew/audit library | |
| Kms.Crypto | Class library | Crypto library | For key generation, encryption and decryption |
| Kms.Core | Class library | Protocol buffers for entire projects, and reusable utilities | |


