#include <SPI.h>
#include <MFRC522.h>

#define SS_PIN 7  // 這個可能需要改成你的接線腳位
#define RST_PIN 9

MFRC522 rfid(SS_PIN, RST_PIN);

void setup() {
    Serial.begin(9600);
    SPI.begin();
    rfid.PCD_Init();
    Serial.println("RFID 準備就緒！");
    
    byte version = rfid.PCD_ReadRegister(rfid.VersionReg);
    Serial.print("Reader : Firmware Version: 0x");
    Serial.println(version, HEX);
    
    if (version == 0x00 || version == 0xFF) {
        Serial.println("⚠️ 無法讀取 MFRC522，請檢查接線！");
    }
}

void loop() {
    // 空迴圈
}
