 #define BAUD 115200

// Must specify this before the include of "ServoEasing.hpp"
#define USE_PCA9685_SERVO_EXPANDER

#include <ServoEasing.hpp>

//servo to control

const int NUM_SERVOS = 3;

const int BASE_SERVO_INDEX = 0;
const int SHOULDER_SERVO_INDEX = 1;
const int ELBOW_SERVO_INDEX = 2;

//ServoEasing Servo1(PCA9685_DEFAULT_ADDRESS, &Wire);

#define START_DEGREE_VALUE  0 // The degree value written to the servo at time of attach.

bool angleSet[NUM_SERVOS];
int angle[NUM_SERVOS];
int servo;


const char J1 = '0'; 
const char J2 = '1';
const char J3 = '2';
const char J4 = '3';
const char J5 = '4';
const char J6 = '5';
const char POSITION = 'p';

String serialInput;
#define BUFFER_SIZE 128
char serialBuffer[BUFFER_SIZE];
char positionBuffer[BUFFER_SIZE];


/*
 * Get the NUM_SERVOS ServoEasing objects for the PCA9685 expander
 * The attach() function inserts them in the ServoEasing::ServoEasingArray[] array.
 */
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
    //if (Servo1.attach(SERVO1_PIN, START_DEGREE_VALUE) == INVALID_SERVO)
    //{
    //  Serial.println(
    // F("Error attaching servo - maybe MAX_EASING_SERVOS=" STR(MAX_EASING_SERVOS) " is to small to hold all servos"));

    //}
    ServoEasing *tServoEasingObjectPtr;
    uint_fast8_t easing = EASE_CUBIC_IN_OUT;
    int servoSpeed = 1;

    for (int i = 0; i < NUM_SERVOS; ++i)
    {
      tServoEasingObjectPtr = new ServoEasing(PCA9685_DEFAULT_ADDRESS, &Wire);
      if (tServoEasingObjectPtr->attach(i, START_DEGREE_VALUE) == INVALID_SERVO)
      {
        Serial.print(F("Address=0x"));
        Serial.print(PCA9685_DEFAULT_ADDRESS, HEX);
        Serial.print(F(" i="));
        Serial.print(i);
        Serial.println(F(
        " Error attaching servo - maybe MAX_EASING_SERVOS=" STR(MAX_EASING_SERVOS) " is to small to hold all servos"));
      }

      ServoEasing::ServoEasingArray[i]->setSpeed(servoSpeed);
      ServoEasing::ServoEasingArray[i]->setEasingType(easing);
      angleSet[i] = false;
      angle[i] = 90;
    }

    writeAllServos(90);
  }
}

void Demo()
{
  int currServo = 0;
  int angle = 135;
  long delayBetweenMoves = 1000;

  

  ServoEasing::ServoEasingArray[currServo]->startEaseToD(angle, 1000);
  delay(delayBetweenMoves);
  
  currServo = 1;
  ServoEasing::ServoEasingArray[currServo]->startEaseToD(angle, 1000);
  delay(delayBetweenMoves);

  currServo = 0;
  angle = 45;

  ServoEasing::ServoEasingArray[currServo]->startEaseToD(angle, 1000);
  delay(delayBetweenMoves);
  
  currServo = 1;
  ServoEasing::ServoEasingArray[currServo]->startEaseToD(angle, 1000);
  delay(delayBetweenMoves);

  currServo = 0;
  angle = 90;

  ServoEasing::ServoEasingArray[currServo]->startEaseToD(angle, 1000);
  delay(delayBetweenMoves);
  
  currServo = 1;
  ServoEasing::ServoEasingArray[currServo]->startEaseToD(angle, 1000);
  //delay(delayBetweenMoves);


}


/*
	* read_command- Command Format
	* <Command>:<Value>:...multiple commands seperated by &  <Command>:<Value>&<Command>:<Value>
	*  multiple command values seperated by  :   e.g. <Command>:<Value>:<Value>
	* <Command>:<Value> e:5 - epression value 5
	* multiple commands seperated by & <Command>:<Value>&<Command>:<Value>
	* e:5&0:4:230 - expression value 5. servo 0 position 4 speed 230
*/

bool ReadSerialCommand(const char * pincomingData)
{

	Serial.print("raw readCommand data: ");
	Serial.println(pincomingData);
 
	char* command_string = strtok((char*)pincomingData, "&");


	while (command_string != 0)
	{
		// Split the command in two values
		Serial.print("Extracted command: ");
		Serial.println(command_string);
		char* separator = strchr(command_string, ':');
		if (separator != 0)
		{
			// Actually split the string in 2: replace ':' with 0
			*separator = 0;
			char curr_command = *command_string;


			Serial.print("Command is ");
			Serial.println(curr_command);

      

			switch (curr_command)
			{
        case J1:
        case J2:
        case J3:
        case J4:
        case J5:
        case J6:
        {
          ++separator;
				  int degree = atoi(separator);
				Serial.print("servo: ");
				Serial.print(curr_command);
				Serial.print(" degree: ");
				Serial.println(degree);

        servo = curr_command - 48; // convert ASCII to int
        angle[servo] = (int)degree;
        angleSet[servo] = true;


        }break;
        case POSITION:
        {
          //modify as more servos are added
           sprintf(positionBuffer, "%d:%d", angle[0], angle[1]);
           Serial.print(positionBuffer);
        }break;
			 
			}
		}
		// Find the next command in input string
		command_string = strtok(0, "&");
	}

	return true;

}



void ReadCommand()
{
  //read the servo index, 0 -15
  servo = Serial.parseInt();

  Serial.print("Servo ");
  Serial.println(servo);

  if(servo == 7)
  {
    Serial.print("Demo mode");
    Demo();
    return;
  }

  while(!Serial.available()) {} //wait for angle

  angle[servo] = Serial.parseInt();
  
  Serial.print("angle: ");
  Serial.println(angle[servo]);

   
   
  angleSet[servo] = true;
   
        

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
     //ReadCommand();

      
     Serial.readBytes(serialBuffer, BUFFER_SIZE);
     ReadSerialCommand(serialBuffer);

   }

   
   for(int i=0;i<NUM_SERVOS;++i)
   {
     if( angleSet[i] == true)
      {
         
        ServoEasing::ServoEasingArray[i]->startEaseToD(angle[i], 1000);
       
        angleSet[i] = false;

      }

   }
   
    
 }