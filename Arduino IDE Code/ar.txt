const int pinButton = 10;
int lastButtonState = HIGH;

void setup() {
  Serial.begin(9600);
  pinMode(pinButton, INPUT_PULLUP);
}

void loop() {
  int currentState = digitalRead(pinButton);

  // Send only when state changes
  if (currentState != lastButtonState) 
  {
    if (currentState == LOW) {
      Serial.write(1);   // Button pressed
    }
    else {
      Serial.write(2);   // Button released
    }

    lastButtonState = currentState;
    delay(20); // debounce
  }
}
