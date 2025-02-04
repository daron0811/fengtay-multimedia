#include <SoftwareSerial.h>
#include <PN532_SWHSU.h>
#include <PN532.h>

SoftwareSerial SWSerial(3, 2); // RX, TX
PN532_SWHSU pn532swhsu(SWSerial);
PN532 nfc(pn532swhsu);

byte nuidPICC[4];

void setup(void) {
  Serial.begin(9600);
  Serial.println("Hello Maker!");

  nfc.begin();
  uint32_t versiondata = nfc.getFirmwareVersion();
  if (!versiondata) {
    Serial.println("Didn't Find PN53x Module");
    while (1); // Halt if module not found
  }

  printVersion(versiondata);
  nfc.SAMConfig(); // Configure the board to read RFID tags
}

void loop() {
  readNFC();
}

void readNFC() {
  uint8_t uid[7] = {0};  // Buffer to store the returned UID
  uint8_t uidLength;      // Length of the UID (4 or 7 bytes depending on ISO14443A card type)

  if (nfc.readPassiveTargetID(PN532_MIFARE_ISO14443A, uid, &uidLength)) {
    printUID(uid, uidLength);
    String tagId = tagToString(uid, uidLength);
    Serial.print(F("tagId is : "));
    Serial.println(tagId);
    delay(2000);  // Reduced delay time to 500ms for faster response
  } else {
    Serial.println("Timed out! Waiting for a card...");
    delay(1000);  // Reduced delay time to 500ms for faster response
  }
}

// Print PN532 Firmware version
void printVersion(uint32_t versiondata) {
  Serial.print("Found chip PN5");
  Serial.println((versiondata >> 24) & 0xFF, HEX);
  Serial.print("Firmware ver. ");
  Serial.print((versiondata >> 16) & 0xFF, DEC);
  Serial.print('.');
  Serial.println((versiondata >> 8) & 0xFF, DEC);
}

// Print UID
void printUID(byte* uid, uint8_t uidLength) {
  Serial.print("UID Length: ");
  Serial.print(uidLength, DEC);
  Serial.println(" bytes");
  Serial.print("UID Value: ");
  for (uint8_t i = 0; i < uidLength; i++) {
    nuidPICC[i] = uid[i];
    Serial.print(" ");
    Serial.print(uid[i], DEC);
  }
  Serial.println();
}

// Convert UID to string
String tagToString(byte* id, uint8_t length) {
  String tagId = "";
  for (uint8_t i = 0; i < length; i++) {
    if (i < length - 1) tagId += String(id[i]) + ".";
    else tagId += String(id[i]);
  }
  return tagId;
}
