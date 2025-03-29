#include <SPI.h>
#include <Adafruit_PN532.h>

// PN532 1 接在 D10 作為 CS
#define PN532_1_SS 10
// PN532 2 接在 D9 作為 CS
#define PN532_2_SS 9

Adafruit_PN532 nfc1(PN532_1_SS);
Adafruit_PN532 nfc2(PN532_2_SS);

void setup() {
  Serial.begin(115200);
  while (!Serial);

  Serial.println("初始化 PN532 NFC 裝置...");

  nfc1.begin();
  nfc2.begin();

  uint32_t versiondata1 = nfc1.getFirmwareVersion();
  if (!versiondata1) {
    Serial.println("模組1 初始化失敗");
  } else {
    Serial.println("模組1 初始化成功");
    nfc1.SAMConfig();
  }

  uint32_t versiondata2 = nfc2.getFirmwareVersion();
  if (!versiondata2) {
    Serial.println("模組2 初始化失敗");
  } else {
    Serial.println("模組2 初始化成功");
    nfc2.SAMConfig();
  }
}

void loop() {
  readTag(nfc1, "模組1");
  readTag(nfc2, "模組2");
  delay(1000);
}

void readTag(Adafruit_PN532 &nfc, String label) {
  uint8_t uid[7];
  uint8_t uidLength;

  if (nfc.readPassiveTargetID(PN532_MIFARE_ISO14443A, uid, &uidLength)) {
    Serial.print(label + " 偵測到卡片 UID: ");
    for (uint8_t i = 0; i < uidLength; i++) {
      Serial.print(uid[i], HEX); Serial.print(" ");
    }
    Serial.println();

    // 嘗試讀取 NDEF 資料（簡單做法，只讀第 4 塊）
    uint8_t data[32];
    if (nfc.mifareultralight_ReadPage(4, data)) {
      Serial.print(label + " NDEF 前4 bytes: ");
      for (int i = 0; i < 4; i++) {
        Serial.print((char)data[i]);
      }
      Serial.println();
    } else {
      Serial.println(label + " 無法讀取 NDEF 資料");
    }
  }
}
