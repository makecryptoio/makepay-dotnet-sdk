# Contributing

## Development

```bash
dotnet restore
dotnet test
dotnet pack src/MakePay/MakePay.csproj -c Release
```

## Pull Requests

- Keep public method names stable once released.
- Add tests for request signing, URL generation, webhook verification, and each
  new endpoint helper.
- Never commit live credentials, webhook secrets, wallet private keys, customer
  data, or production payload dumps.
- Keep the SDK aligned with the OpenAPI contract in
  `makecryptoio/makepay-openapi`.
