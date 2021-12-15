#define BAUD 115200

 

#include <ServoEasing.hpp>

//servo to control

const int NUM_SERVOS = 3;

const int BASE_SERVO_INDEX = 0;
const int SHOULDER_SERVO_INDEX = 1;
const int ELBOW_SERVO_INDEX = 2;

#define START_DEGREE_VALUE  90 // The degree value written to the servo at time of attach.

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


//map joints to ESP32 GPIO
#define S0_PIN 15
#define S1_PIN 16
#define S2_PIN 17
#define S3_PIN 18
#define S4_PIN 19
#define S5_PIN 20

 
 

short servoPins[] = {S0_PIN,S1_PIN,S2_PIN,S3_PIN,S4_PIN,S5_PIN};


/*
 * Get the NUM_SERVOS ServoEasing objects for the PCA9685 expander
 * The attach() function inserts them in the ServoEasing::ServoEasingArray[] array.
 */
void InitServos()
{
    ServoEasing *tServoEasingObjectPtr;
    uint_fast8_t easing = EASE_CUBIC_IN_OUT;
    int servoSpeed = 1;

    for (int i = 0; i < NUM_SERVOS; ++i)
    {
      tServoEasingObjectPtr = new ServoEasing();
      Serial.print("attaching servo to pin ");
      Serial.println(servoPins[i]);

      if (tServoEasingObjectPtr->attach(servoPins[i], START_DEGREE_VALUE) == INVALID_SERVO)
      {
        Serial.print(F(" i="));
        Serial.print(i);
        Serial.println(F(
        " Error attaching servo - maybe MAX_EASING_SERVOS=" STR(MAX_EASING_SERVOS) " is to small to hold all servos"));
      }

      ServoEasing::ServoEasingArray[i]->setSpeed(servoSpeed);
      ServoEasing::ServoEasingArray[i]->setEasingType(easing);
      angleSet[i] = false;
      angle[i] = START_DEGREE_VALUE;
    }

    writeAllServos(START_DEGREE_VALUE);
}
/*
	* read_command- Command Format
	* <Command>:<Value>:...multiple commands seperated by &  <Command>:<Value>&<Command>:<Value>
	*  multiple command values seperated by  :   e.g. <Command>:<Value>:<Value>
	* <Command>:<Value> e:5 - epression value 5
	* multiple commands seperated by & <Command>:<Value>&<Command>:<Value>
	* e:5&0:4:230 - expression value 5. servo 0 position 4 speed 230
*/

bool ReadSerialCommand(const char *pincomingData)
{

    Serial.print("raw readCommand data: ");
    Serial.println(pincomingData);

    char *command_string = strtok((char *)pincomingData, "&");

    while (command_string != 0)
    {
        // Split the command in two values
        Serial.print("Extracted command: ");
        Serial.println(command_string);
        char *separator = strchr(command_string, ':');
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
            }
            break;
            case POSITION:
            {
                //modify as more servos are added
                sprintf(positionBuffer, "%d:%d", angle[0], angle[1]);
                Serial.print(positionBuffer);
            }
            break;
            }
        }
        // Find the next command in input string
        command_string = strtok(0, "&");
    }

    return true;
}

void setup()
{
    Serial.begin(115200);
    InitServos();
    // Wait for servo to reach start position.
    delay(500);
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

    for (int i = 0; i < NUM_SERVOS; ++i)
    {
        if (angleSet[i] == true)
        {

            ServoEasing::ServoEasingArray[i]->startEaseToD(angle[i], 1000);

            angleSet[i] = false;
        }
    }
}