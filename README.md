# LoopringSmartWalletRecoveryPhraseExtractor
Extract the Recovery Phrase for a Loopring Smart Wallet via the Loopring Migration QR Code.

## Important
DO NOT SHARE ANYTHING GENERATED BY THIS APP OR THE QR MIGRATON CODE WITH ANYONE AT ALL

## Requirements
1. Visual Studio
2. Loopring Migration QR Code - DO NOT SHARE THIS WITH ANYONE AT ALL
3. Loopring App Passcode - DO NOT SHARE THIS WITH ANYONE AT ALL

## How to use

1. Generate the Migration QR Code via the Loopring Smart Wallet app. You then need to get the decoded JSON Text data from this QR code, you can use any QR reader app to extract this text out from the QR Code Image. The extracted JSON text should look like below. DO NOT SHARE THE QR CODE OR THE RESULTING DECODED TEXT WITH ANYONE AT ALL!!!

```json
{"wallet":"0x99","iv":"2IcZe","mnemonic":"uvkZ","ens":"xxx.loopring.eth","isCounterFactual":false,"register":"61,","type":"LoopringWalletSmart","setting":3232,"salt":"ikq","network":"ETHEREUM"}
```

2. Run this solution in Visual Studio.

3. Enter the extracted QR Code Text into the first prompt. DO NOT SHARE THE QR CODE TEXT WITH ANYONE AT ALL!!!

4. Enter your Loopring App Passcode into the second prompt. DO NOT SHARE THIS PASSCODE WITH ANYONE AT ALL!!!

5. Your recovery passphrase should then be displayed. DO NOT SHARE THIS WITH ANYONE AT ALL

6. You can import this recovery passphrase into metamask. The 1st account created by the import is your OWNER wallet of the Loopring Smart Wallet. You can then extract the L1 private key for this OWNER wallet to use to sign requests with the Loopring API that need ECDSA. You can use this L1 Private Key of the OWNET wallet with Loopring Airdrop tools like [Maize](https://github.com/cobmin/Maize)

