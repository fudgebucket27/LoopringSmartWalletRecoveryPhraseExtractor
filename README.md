# LoopringSmartWalletRecoveryPhraseExtractor
## What is this?
This is a tool to extract the Recovery Phrase and L1 Private Key for the OWNER wallet of a Loopring Smart Wallet via the Loopring Migration QR Code. 

## Important
DO NOT SHARE ANYTHING GENERATED BY THIS TOOL OR THE LOOPRING QR MIGRATON CODE WITH ANYONE AT ALL

## Requirements
1. Visual Studio 2022 - only needed if you are building from this source code yourself. Otherwise grab a precompiled release [here](https://github.com/fudgebucket27/LoopringSmartWalletRecoveryPhraseExtractor/releases)
2. Loopring Migration QR Code - DO NOT SHARE THIS WITH ANYONE AT ALL
3. Loopring App Passcode - DO NOT SHARE THIS WITH ANYONE AT ALL
4. Layer 1 Active for your Loopring Smart Wallet

## How to use

1. Generate the Migration QR Code via the Loopring Smart Wallet app. Save this QR Code in a location that this application will have access to. DO NOT SHARE THE QR CODE WITH ANYONE AT ALL!!!

2. If building from source, clone this repo and run this solution in Visual Studio. Otherwise grab a precompiled release  [here](https://github.com/fudgebucket27/LoopringSmartWalletRecoveryPhraseExtractor/releases),unzip and run the executable file.

3. Enter the QR Code's full image path (e.g., C:\Users\Documents\qrcode.png) into the first prompt. DO NOT SHARE THE QR CODE TEXT WITH ANYONE AT ALL!!!

4. Enter your Loopring App Passcode into the second prompt. DO NOT SHARE THIS PASSCODE WITH ANYONE AT ALL!!!

5. Your recovery passphrase for the OWNER wallet should then be displayed. DO NOT SHARE THIS WITH ANYONE AT ALL

6. Your L1 Private key of the OWNER wallet will then be displayed. You can use this L1 Private Key of the OWNER wallet to sign requests for your Loopring Smart Wallet with the Loopring API that need ECDSA. You can also use this L1 Private Key of the OWNER wallet with Loopring Airdrop tools like [Maize](https://github.com/cobmin/Maize) for your Loopring Smart Wallet. DO NOT SHARE THE L1 PRIVATE KEY WITH ANYONE AT ALL. A MALICIOUS USER COULD DO A WITHDRAWAL/TRANSFER OF YOUR ASSETS FROM LOOPRING LAYER 2 INTO ANY LOOPRING LAYER 2/ETHEREUM LAYER 1 ADDRESS WITH IT.

# Credits
Huge thanks to Folays in the Loopring discord for the original process which I converted into C#. Here's his [post](https://discord.com/channels/488848270525857792/700743843921920073/1089542488240439498) where he figured this out!
![image](https://github.com/fudgebucket27/LoopringSmartWalletRecoveryPhraseExtractor/assets/5258063/4a4bc2fd-82c2-440e-858f-cd6f2c4d961d)
