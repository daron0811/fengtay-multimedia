#include <Wire.h>
#include <PN532_I2C.h>
#include <PN532.h>
#include <NfcAdapter.h>

PN532_I2C pn532_i2c(Wire);
NfcAdapter nfc(pn532_i2c);

void setup() {
    Serial.begin(115200);
    Serial.println("NDEF Write Test");

    nfc.begin();
}

void loop() {
    Serial.println("請將 NFC 標籤靠近...");

    if (nfc.tagPresent()) {
        NdefMessage message;
        message.addTextRecord("西瓜");  // 這裡可以改成 Unity 傳來的數據

        bool success = nfc.write(message);
        if (success) {
            Serial.println("✅ NDEF 訊息已寫入 NFC 卡片！");
        } else {
            Serial.println("❌ 無法寫入 NFC 卡片！");
        }

        delay(2000);  // 等待 2 秒，避免重複寫入
    }
}
