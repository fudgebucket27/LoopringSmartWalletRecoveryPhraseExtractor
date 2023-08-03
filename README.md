# LoopringSmartWalletRecoveryPhraseExtractor
Extract the Recovery Phrase for a Loopring Smart Wallet via the Loopring Migration QR Code.

## Important
This is still a work in progress. I'm still figuring out how to get the derived account that Loopring generates via the recovery phrase...

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

# TO DO
Figure out how to get the derived account address / private key that Loopring uses, as we are currently getting the first account index.
