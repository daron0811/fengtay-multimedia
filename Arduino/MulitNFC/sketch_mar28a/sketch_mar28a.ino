#include <SPI.h>
#include <Adafruit_PN532.h>

#define PN532_CS_1 10
#define PN532_CS_2 9

Adafruit_PN532 nfc1(PN532_CS_1);
Adafruit_PN532 nfc2(PN532_CS_2);

int activeReader = 0;  // 0 = nfc1, 1 = nfc2
unsigned long lastSwitchTime = 0;
const unsigned long switchInterval = 3000;
bool logPrinted = false;

// String command = "R1";

void setup() {
  Serial.begin(115200);
  // Serial.println("🔁 雙 PN532 輪流偵測（每秒切換）");
  String json = buildJson(201, "Start Arduino", "", String(activeReader));
  Serial.println(json);

  initPN532(nfc1, "PN532_1");
  initPN532(nfc2, "PN532_2");
}

void checkSerialCommand() {
  if (Serial.available()) {
    String command = Serial.readStringUntil('\n');
    command.trim();
    if (command == "R1") {
      activeReader = 0;
      // Serial.println("✅ 切換到 PN532_1");
      String json = buildJson(203, "Switch PN532_1", "", "R1");
      Serial.println(json);
      // String json = "{\"status\":203, \"msg\":\"Switch PN532_1\", \"nfc\":\"\", \"reader\":\"" + command + "\"}";
    } else if (command == "R2") {
      activeReader = 1;
      String json = buildJson(203, "Switch PN532_2", "", "R2");
      Serial.println(json);
      // Serial.println("✅ 切換到 PN532_2");
    }
  }
}


void loop() {
  // unsigned long currentTime = millis();

  // // 每秒切換一次偵測對象
  // if (currentTime - lastSwitchTime >= switchInterval) {
  //   activeReader = (activeReader + 1) % 2;
  //   lastSwitchTime = currentTime;
  //   logPrinted = false;
  // }

  // if (!logPrinted) {
  //   if (activeReader == 0) {
  //     Serial.println("🟢 現在由 PN532_1 偵測中...");
  //   } else {
  //     Serial.println("🟡 現在由 PN532_2 偵測中...");
  //   }
  //   logPrinted = true;
  // }

  checkSerialCommand();

  // if (activeReader == 0) {
  //   Serial.println("🟢 現在由 PN532_1 偵測中...");
  // } else {
  //   Serial.println("🟡 現在由 PN532_2 偵測中...");
  // }


  if (activeReader == 0) {
    readNFC_nonBlocking(nfc1, "PN532_1");
  } else {
    readNFC_nonBlocking(nfc2, "PN532_2");
  }
}

// 初始化
void initPN532(Adafruit_PN532& nfc, String readerName) {
  nfc.begin();
  uint32_t versiondata = nfc.getFirmwareVersion();
  if (!versiondata) {
    //String json = "{\"status\":202, \"msg\":\"" + readerName + " : Success Connect\", \"nfc\":\"\", \"reader\":\"\"}";
    String json = buildJson(202, readerName + " : Success Connect", "", String(activeReader));
    Serial.println(json);
    // Serial.println("❌ " + readerName + " 未偵測到！");
  } else {
    // Serial.print("✅ " + readerName + " 已連接！ Firmware: ");
    // String json = "{\"status\":202, \"msg\":\"" + readerName + " : Success Connect\", \"nfc\":\"\", \"reader\":\"\"}";
    String json = buildJson(202, readerName + " : Success Connect", "", String(activeReader));
    Serial.println(json);
    // Serial.println(versiondata, HEX);
    nfc.SAMConfig();
  }
}

String buildJson(int status, String msg, String nfc, String reader) {
  return "{\"status\":" + String(status) + ", \"msg\":\"" + msg + "\", \"nfc\":\"" + nfc + "\", \"reader\":\"" + reader + "\"}";
}

// // 非阻塞 NFC 讀取
// void readNFC_nonBlocking(Adafruit_PN532& nfc, String readerName) {
//   uint8_t uid[7];
//   uint8_t uidLength;

//   // 非阻塞讀取，加上 timeout（例如10毫秒）
//   if (nfc.readPassiveTargetID(PN532_MIFARE_ISO14443A, uid, &uidLength, 1000)) {
//     Serial.print(readerName + " Find UID：");
//     for (uint8_t i = 0; i < uidLength; i++) {
//       Serial.print(" 0x");
//       Serial.print(uid[i], HEX);
//     }
//     Serial.println("");

//     uint8_t pageData[4];
//     if (nfc.ntag2xx_ReadPage(4, pageData)) {
//       Serial.print(readerName + " ➜ Page 4：");
//       for (int i = 0; i < 4; i++) {
//         Serial.print(" 0x");
//         Serial.print(pageData[i], HEX);
//       }
//       Serial.println("");
//     } else {
//       Serial.println(readerName + " ❌ 無法讀取 Page 4");
//     }
//     delay(1000);
//   }
// }


void readNFC_nonBlocking(Adafruit_PN532& nfc, String readerName) {
  uint8_t uid[7];
  uint8_t uidLength;

  // 非阻塞讀取，加上 timeout（例如 1000 毫秒）
  if (nfc.readPassiveTargetID(PN532_MIFARE_ISO14443A, uid, &uidLength, 1000)) {
    // 建立 UID 字串
    String uidStr = "";
    for (uint8_t i = 0; i < uidLength; i++) {
      if (i > 0) uidStr += " ";
      uidStr += "0x";
      if (uid[i] < 0x10) uidStr += "0";
      uidStr += String(uid[i], HEX);
    }




    // 讀取 page 4 ~ 15（共 12 頁 × 4 byte = 48 byte）
    // uint8_t data[48];
    // for (int i = 0; i < 12; i++) {
    //   if (!nfc.ntag2xx_ReadPage(4 + i, &data[i * 4])) {
    //     Serial.println("❌ 讀取失敗 Page " + String(4 + i));
    //     return;
    //   }
    // }

    // // 印出十六進位資料（debug 用）
    // Serial.println("📦 原始內容（Hex）：");
    // for (int i = 0; i < 48; i++) {
    //   Serial.print("0x");
    //   if (data[i] < 0x10) Serial.print("0");
    //   Serial.print(data[i], HEX);
    //   Serial.print(" ");
    // }
    // Serial.println();

    // // 嘗試解析 NDEF Payload（找出文字內容）
    // Serial.println("🔍 嘗試解析 NDEF...");
    // int textStartIndex = 0;

    // // 找 NDEF 開頭
    // if (data[0] == 0x03) {
    //   uint8_t length = data[1];  // NDEF 資料長度
    //   uint8_t typeLength = data[3];
    //   uint8_t payloadLength = data[4];
    //   uint8_t languageLength = data[6]; // 通常是 2 (en)
    //   textStartIndex = 7 + languageLength;

    //   String message = "";
    //   for (int i = textStartIndex; i < 7 + payloadLength; i++) {
    //     message += (char)data[i];
    //   }

    //   Serial.println("✅ 讀到內容: " + message);
    // } else {
    //   Serial.println("⚠️ 沒有發現 NDEF 資料");
    // }


    // 建立 JSON 字串（status 可改 400 → 200 為成功）
    // String json = "{\"status\":200, \"msg\":\"成功\", \"nfc\":\"" + uidStr + "\", \"reader\":\"" + readerName + "\"}";
    String json = buildJson(202, readerName + " : Success Sensor", uidStr, String(activeReader));
    // 回傳給 Unity（或其他串列接收端）
    Serial.println(json);

    // 延遲避免重複掃到
    delay(1000);
  }
}





// #include <SPI.h>
// #include <Adafruit_PN532.h>

// #define PN532_CS_1 10
// #define PN532_CS_2 9

// Adafruit_PN532 nfc1(PN532_CS_1);
// Adafruit_PN532 nfc2(PN532_CS_2);

// int activeReader = 0; // 0 = nfc1, 1 = nfc2

// void setup() {
//   Serial.begin(115200);
//   Serial.println("🔌 等待 Unity 指令控制 PN532");

//   initPN532(nfc1, "PN532_1");
//   initPN532(nfc2, "PN532_2");
// }

// void loop() {
//   checkSerialCommand();

//   if (activeReader == 0) {
//     readNFC_nonBlocking(nfc1, "PN532_1");
//   } else if (activeReader == 1) {
//     readNFC_nonBlocking(nfc2, "PN532_2");
//   }

//   delay(50);  // 輕微延遲，避免跑太快，但不阻塞太久
// }

// void checkSerialCommand() {
//   if (Serial.available()) {
//     String command = Serial.readStringUntil('\n');
//     command.trim();

//     if (command == "R1") {
//       activeReader = 0;
//       Serial.println("✅ 切換到 PN532_1");
//     } else if (command == "R2") {
//       activeReader = 1;
//       Serial.println("✅ 切換到 PN532_2");
//     }
//   }
// }

// void initPN532(Adafruit_PN532 &nfc, const String &readerName) {
//   nfc.begin();
//   uint32_t versiondata = nfc.getFirmwareVersion();
//   if (!versiondata) {
//     Serial.println("❌ " + readerName + " 未偵測到！");
//   } else {
//     Serial.print("✅ " + readerName + " 已連接！Firmware: ");
//     Serial.println(versiondata, HEX);
//     nfc.SAMConfig();
//   }
// }

// void readNFC_nonBlocking(Adafruit_PN532 &nfc, const String &readerName) {
//   Serial.println(readerName + " 偵測中...");

//   uint8_t uid[7];
//   uint8_t uidLength;

//   // 非阻塞方式，timeout 10 毫秒
//   if (nfc.readPassiveTargetID(PN532_MIFARE_ISO14443A, uid, &uidLength, 10)) {
//     Serial.print(readerName + " UID:");
//     for (uint8_t i = 0; i < uidLength; i++) {
//       if (uid[i] < 0x10) Serial.print(" 0"); // 補 0
//       else Serial.print(" ");
//       Serial.print(uid[i], HEX);
//     }
//     Serial.println(); // 結尾換行，讓 Unity .ReadLine() 能正常解析
//   }
// }
