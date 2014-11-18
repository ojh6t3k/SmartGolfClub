#include <UnityRobot.h>
#include <DigitalPin.h>
#include <AnalogPin.h>

DigitalPin digitalPin2(2, 2); // id, pin
DigitalPin digitalPin3(3, 3); // id, pin
DigitalPin digitalPin4(4, 4); // id, pin
DigitalPin digitalPin5(5, 5); // id, pin
DigitalPin digitalPin6(6, 6); // id, pin
DigitalPin digitalPin7(7, 7); // id, pin
DigitalPin digitalPin8(8, 8); // id, pin
DigitalPin digitalPin9(9, 9); // id, pin
DigitalPin digitalPin10(10, 10); // id, pin
DigitalPin digitalPin11(11, 11); // id, pin
DigitalPin digitalPin12(12, 12); // id, pin
DigitalPin digitalPin13(13, 13); // id, pin

AnalogPin analogPin0(14, A0);
AnalogPin analogPin1(15, A1);
AnalogPin analogPin2(16, A2);
AnalogPin analogPin3(17, A3);
AnalogPin analogPin4(18, A4);
AnalogPin analogPin5(19, A5);

void setup()
{
  UnityRobot.begin(57600);
  UnityRobot.attachModule((UnityModule*)&digitalPin2);
  UnityRobot.attachModule((UnityModule*)&digitalPin3);
  UnityRobot.attachModule((UnityModule*)&digitalPin4);
  UnityRobot.attachModule((UnityModule*)&digitalPin5);
  UnityRobot.attachModule((UnityModule*)&digitalPin6);
  UnityRobot.attachModule((UnityModule*)&digitalPin7);
  UnityRobot.attachModule((UnityModule*)&digitalPin8);
  UnityRobot.attachModule((UnityModule*)&digitalPin9);
  UnityRobot.attachModule((UnityModule*)&digitalPin10);
  UnityRobot.attachModule((UnityModule*)&digitalPin11);
  UnityRobot.attachModule((UnityModule*)&digitalPin12);
  UnityRobot.attachModule((UnityModule*)&digitalPin13);
  
  UnityRobot.attachModule((UnityModule*)&analogPin0);
  UnityRobot.attachModule((UnityModule*)&analogPin1);
  UnityRobot.attachModule((UnityModule*)&analogPin2);
  UnityRobot.attachModule((UnityModule*)&analogPin3);
  UnityRobot.attachModule((UnityModule*)&analogPin4);
  UnityRobot.attachModule((UnityModule*)&analogPin5);
}

void loop()
{
  UnityRobot.process();
}
