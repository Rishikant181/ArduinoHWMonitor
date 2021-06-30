#include "rgb_lcd.h"

rgb_lcd lcd;
int index;
String data;
String line0;
String line1;

void setup() {
    lcd.begin(16, 2);
    lcd.setRGB(0, 10, 0);
    Serial.begin(500000);
}

void loop() {
    while(Serial.available())  {
        data = Serial.readString();
        index = data.indexOf('\n');
        line0 = data.substring(0, index);
        line1 = data.substring(index + 1);
        lcd.setCursor(0, 0);
        lcd.print(line0);
        lcd.setCursor(0, 1);
        lcd.print(line1);
    }
}
