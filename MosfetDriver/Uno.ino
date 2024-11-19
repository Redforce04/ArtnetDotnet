// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PiZero2W
//    Project:          MosfetDriver
//    FileName:         Example.cs
//    Author:           Redforce04#4091
//    Revision Date:    11/04/2024 19:11
//    Created Date:     11/04/2024 19:11
// -----------------------------------------
#define serialBaud 9600
#define pwmPin1 3
#define pwmPin2 5
#define pwmPin3 6
#define pwmPin4 9

void setup() {
  DDRB |= (1<<DDB1);                        // Set Pin B1 as Output (OC1A)
    
  //OC0A PD6, Chip12, 6
  //OC0B PD5, Chip11, 5
  //OC1A, PD1, Chip15, 1,
  //OC1B, PD2, Chip16, 2,
  //OC2A, PD11, Chip17, 11
  //OC2B, PD3, Chip5, 3

  // Set Ports to Output mode
  // PinMode(1, OUTPUT); //  15
  // pinMode(2, OUTPUT); // 16
  pinMode(3, OUTPUT); //  5
  pinMode(11, OUTPUT); // 17
  pinMode(6, OUTPUT);  // 12
  pinMode(5, OUTPUT);  // 11

  // Set Timer 1 & 2 to Fast PWM
  // TCCR1A = _BV(COM1A1) | _BV(COM1B1) | _BV(WGM12); // Fast PWM mode    
  TCCR2A = _BV(COM2A1) | _BV(COM2B1) | _BV(WGM21) | _BV(WGM20);
  TCCR2B = _BV(CS22);
  TCCR0A = _BV(COM2A1) | _BV(COM2B1) | _BV(WGM21) | _BV(WGM20);
  TCCR0B = _BV(CS22);

  // TCCR0A = (1|0|1|0|0|0|1|1);
  // TCCR0B = (0|0|0|0|1|0|1|1);
  // TCCR1A = ;    // Timer 1 is 16 bit, not 8bit
  // TCCR1B = ;
  // TCCR2A = (1|0|1|0|0|0|1|1);
  // TCCR2B = (0|0|0|0|1|1|0|0);
  OCR2A = 255; // 11
  OCR2B = 50; // 3
  OCR0A = 10; // 6
  OCR0B = 1;  // 5
  // ICR1 = 8191;                                           // Set the number of PWM steps
  Serial.begin(serialBaud);                                      // Start communication with serial monitor
}

void loop() {
 String str = Serial.readStringUntil('\n');
 char *array;
 int len = str.length();
 String lastLine = "";
 int curVal = 0;
 
for (int i = 0; i <= len; i++)
 {
    if(str[i] == ' '){
      int val = lastLine.toInt();
      switch(curVal){
        case 0:
          OCR2A = val;
          break;
        case 1:
          OCR2B = val;
          break;
        case 2:
          OCR0A = val;
          break;
        case 3:
          OCR0B = val;
          break;
        default:
          break;
      }
      lastLine = "";
      Serial.print(String(curVal) + ": " + String(val) + ", ");
      curVal += 1;
      continue;
    }
    lastLine = lastLine + str[i];
 }
 Serial.println("");                                  // Print current PWM setting on serial monitor

 // Serial.println(str + "  [" + String(len) + "]");                                  // Print current PWM setting on serial monitor
}