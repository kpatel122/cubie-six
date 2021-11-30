 #define BAUD 115200

// Must specify this before the include of "ServoEasing.hpp"
#define USE_PCA9685_SERVO_EXPANDER

#include <ServoEasing.hpp>

//servo to control
const int SERVO1_PIN = 0;

ServoEasing Servo1(PCA9685_DEFAULT_ADDRESS, &Wire);

#define START_DEGREE_VALUE  0 // The degree value written to the servo at time of attach.

bool angleSet = false;
int angle=0;

void InitServos()
{

   Wire.begin();  // Starts with 100 kHz. Clock will eventually be increased at first attach() except for ESP32.
 
    Wire.setWireTimeout(); // Sets default timeout of 25 ms.


  if (checkI2CConnection(PCA9685_DEFAULT_ADDRESS, &Serial)) 
  {
    Serial.println(F("PCA9685 expander not connected"));
         
  } 
  else 
  {
      Serial.println(F("Attach servo to port 0 of PCA9685 expander"));
      /************************************************************
      * Attach servo to pin and set servos to start position.
      * This is the position where the movement starts.
      *
      * Check at least the last call to attach()
      ***********************************************************/
      if (Servo1.attach(SERVO1_PIN, START_DEGREE_VALUE) == INVALID_SERVO) 
      {
        Serial.println(
        F("Error attaching servo - maybe MAX_EASING_SERVOS=" STR(MAX_EASING_SERVOS) " is to small to hold all servos"));
              
      }
    }

    Servo1.setSpeed(10);
    Servo1.setEasingType(EASE_CUBIC_IN_OUT);
 
  
}

void ReadCommand()
{
  //read the servo index, 0 -15
  angle = Serial.parseInt();
  
  Serial.print("global angle: ");
  Serial.println(angle);

   
   
  angleSet = true;
   
        

}

 void setup()
 {
    Serial.begin(BAUD);
    
    InitServos();

    

 }

 void loop()
 {
    //command sequencegoes servo index then angle
   if (Serial.available() > 0) 
   {
     ReadCommand();        
   }

    if( angleSet == true)
    {
         
      Servo1.startEaseToD(angle, 1000);
      angleSet = false;

    }
 }