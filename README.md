# LoopringSmartWalletRecoveryPhraseExtractor
Extract the Recovery Phrase for a Loopring Smart Wallet via the Migration QR Code.

# # How to use

1. Get the Migration QR Code JSON Text, you need to use a QR reader app to extract this text out. The extracted JSON text should look like below

```json
{"wallet":"0x99","iv":"2IcZe","mnemonic":"uvkZ","ens":"xxx.loopring.eth","isCounterFactual":false,"register":"61,","type":"LoopringWalletSmart","setting":3232,"salt":"ikq","network":"ETHEREUM"}
```

2. Run this solution in Visual Studio.

3. Enter the extracted QR Code Text into the first prompt

4. Enter your Loopring App Passcode

5. Your recovery passphrase should then be displayed

# TO DO
Figure out how to get the derived account address / private key that Loopring uses as we are currently getting the first account index.
