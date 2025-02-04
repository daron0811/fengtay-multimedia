#include <SPI.h>
#include <MFRC522.h>

#define SS_PIN 10  // 這個可能需要改成你的接線腳位
#define RST_PIN 9

MFRC522 rfid(SS_PIN, RST_PIN);

void setup() {
  Serial.begin(9600);
  SPI.begin();
  rfid.PCD_Init();
  Serial.println("🎯 RFID 準備就緒！");

  byte version = rfid.PCD_ReadRegister(rfid.VersionReg);
  Serial.print("📖 讀卡機 Firmware 版本: 0x");
  Serial.println(version, HEX);

  if (version == 0x00 || version == 0xFF) {
    Serial.println("⚠️ 無法讀取 MFRC522，請檢查接線！");
  }
}

void loop() {
  // 🔴 檢查讀卡機是否仍在運作
  byte version = rfid.PCD_ReadRegister(rfid.VersionReg);
  if (version == 0x00 || version == 0xFF) {
    Serial.println("🚨 錯誤！讀卡機可能斷訊，正在重新初始化...");
    rfid.PCD_Init();  // 嘗試重新初始化
    delay(500);
    return;
  }

  // 🟢 1️⃣ 檢查是否有卡片靠近
  if (!rfid.PICC_IsNewCardPresent() || !rfid.PICC_ReadCardSerial()) {
    Serial.println("⚠️ 沒有偵測到新卡片...");
    delay(500);
    return;
  }

  // 🎉 讀取成功，顯示卡片 UID
  Serial.print("🔑 卡片 UID: ");
  for (byte i = 0; i < rfid.uid.size; i++) {
    Serial.print(rfid.uid.uidByte[i] < 0x10 ? " 0" : " ");
    Serial.print(rfid.uid.uidByte[i], HEX);
  }
  Serial.println();

  // 🟢 2️⃣ 取得卡片類型
  MFRC522::PICC_Type piccType = rfid.PICC_GetType(rfid.uid.sak);
  Serial.print("📌 卡片類型: ");
  Serial.println(rfid.PICC_GetTypeName(piccType));

  // 🟢 3️⃣ 嘗試讀取卡片數據（區塊 4）
  byte buffer[18];
  byte size = sizeof(buffer);
  MFRC522::StatusCode status = rfid.MIFARE_Read(4, buffer, &size);

  if (status == MFRC522::STATUS_OK) {
    Serial.println("📝 嘗試讀取卡片數據...");
    bool isBlank = true;

    // 🔍 檢查是否為空白卡（所有數據都是 0x00 或 0xFF）
    for (byte i = 0; i < 16; i++) {
      if (buffer[i] != 0x00 && buffer[i] != 0xFF) {
        isBlank = false;
        break;
      }
    }

    if (isBlank) {
      Serial.println("⚠️ 這張卡片可能是 **空白卡**！請確認是否需要寫入數據。");
    } else {
      Serial.println("✅ 這張卡片已寫入數據！");
    }
  } else {
    Serial.println("⚠️ 無法讀取卡片數據，可能是未初始化的空白卡！");
  }

  // 停止對卡片的操作
  rfid.PICC_HaltA();
  rfid.PCD_StopCrypto1();

  // 延遲 1 秒避免過多 log
  delay(1000);
}
