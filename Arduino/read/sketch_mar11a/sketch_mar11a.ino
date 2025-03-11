#include <Wire.h>
#include <PN532_I2C.h>
#include <PN532.h>
#include <NfcAdapter.h>

PN532_I2C pn532_i2c(Wire);
NfcAdapter nfc(pn532_i2c);

void setup() {
    Serial.begin(115200);
    Serial.println("NDEF Read Mode");
    nfc.begin();
}

void loop() {
    Serial.println("請將 NFC 標籤靠近...");

    if (nfc.tagPresent()) {
        NfcTag tag = nfc.read();
        Serial.println("✅ NFC 標籤已讀取！");

        if (tag.hasNdefMessage()) {
            NdefMessage message = tag.getNdefMessage();
            Serial.print("NDEF 訊息數量: ");
            Serial.println(message.getRecordCount());

            for (int i = 0; i < message.getRecordCount(); i++) {
                NdefRecord record = message.getRecord(i);
                int payloadLength = record.getPayloadLength();
                byte payload[payloadLength];
                record.getPayload(payload);

                Serial.print("讀取內容: ");
                for (int j = 0; j < payloadLength; j++) {
                    Serial.write(payload[j]);  // 顯示在 Serial
                }
                Serial.println();
            }
        } else {
            Serial.println("⚠️ 此 NFC 卡無 NDEF 訊息");
        }
        delay(2000);  // 避免重複讀取
    }
}
