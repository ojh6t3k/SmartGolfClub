#include "UnityRobot.h"
#include "imuModule.h"
#include "AnalogPin.h"
#include "ToneModule.h"
#include "mpu.h"
#include "pitches.h"
#include "I2Cdev.h"


#define FLEX_SENSOR           A0        // Flex sensor (Spectra Symbol) to analog pin 0
#define POTENTIOMETER         A1        // SoftPot potentiometer (Spectra Symbol) to analog pin 1
#define BUZZER                10        // Buzzer to digital pin 10

signed char imu_orientation[9] = { 1, 0, 0
				,0, 1, 0
				,0, 0, 1};


imuModule imu(1);
AnalogPin softPot(2, POTENTIOMETER);
ToneModule toneModule(3, BUZZER);

void setup()
{
  pinMode(BUZZER, OUTPUT);  
  Fastwire::setup(400, 0);
  
  if(!mympu_open(200, imu_orientation))
  {
    tone(BUZZER, NOTE_C7, 25);          // Generates a square wave of the specified frequency on a pin
    delay(50);
    tone(BUZZER, NOTE_E7, 25);          // Generates a square wave of the specified frequency on a pin
    delay(50);
    tone(BUZZER, NOTE_G7, 250);         // Generates a square wave of the specified frequency on a pin
  }
  
  UnityRobot.begin(115200);
  UnityRobot.attachModule((UnityModule*)&imu);
  UnityRobot.attachModule((UnityModule*)&softPot);
  UnityRobot.attachModule((UnityModule*)&toneModule);
}

void loop()
{
  UnityRobot.process();
  
  if(!mympu_update()) // Successful MPU9150 DMP read
  {
    imu.SetQuarternion(mympu.xyzw[0], mympu.xyzw[1], mympu.xyzw[2], mympu.xyzw[3]);
  }
}
