#include <SoftwareSerial.h>

// RX = 8, TX = 10
SoftwareSerial espSerial(8, 10);

void setup() {
  espSerial.begin(9600);
}

void loop() {
  int gas = analogRead(A0);
  espSerial.println(gas);
  delay(1000);
}
