const int in1 = 2;
const int in2 = 3;
const int in3 = 4;
const int in4 = 5;
int rychlost = 20;
String GetCode; 
String SendData;
int PwrOnPin = 9;
int InitializedPin = 8;
int ShatterOpenPin = 7;
int IntCode = 0;
boolean ShutterIsClosed = true;
boolean ReadCheck = false;
boolean CycleIsLoaded = false;
unsigned long CycleStart;
unsigned long CycleTime;
unsigned long PulseStart;
unsigned long PulseTime;
union ArrayToInteger {
 byte array[2];
 int integer;
};
ArrayToInteger Ati;
typedef struct 
{
  int CycleDuration;
  int OnTime;
  int OffTime;
} CycleItem;
int CycleNumber;
int ItemsNumber;
CycleItem CycleItems[10];
void setup() 
{
 pinMode(PwrOnPin,OUTPUT);
 digitalWrite(PwrOnPin, HIGH);
 pinMode(InitializedPin,OUTPUT);
 pinMode(ShatterOpenPin,OUTPUT);
 pinMode(13,OUTPUT);
 pinMode(2,OUTPUT);
 pinMode(3,OUTPUT);
 pinMode(4,OUTPUT);
 pinMode(5,OUTPUT);
 Serial.begin(115200);
 InitializePort();
}

void loop() 
{ 
    GetCode = Serial.readString();      
    IntCode = GetCode.toInt();
    switch (IntCode)
    {
      case 1989: 
          Serial.println("1603");
          digitalWrite(InitializedPin, HIGH);
          delay(100);
          digitalWrite(InitializedPin, LOW);
          delay(100);
          digitalWrite(InitializedPin, HIGH);
          delay(100);
          digitalWrite(InitializedPin, LOW);
          delay(100);
          digitalWrite(InitializedPin, HIGH);
          delay(100);
          digitalWrite(InitializedPin, LOW);
          break;
      case 1111:
          ShutterOpen();        
          break;
      case 1000:
         ShutterClose();    
         //  digitalWrite(13, LOW);    
          break;
      case 1001:
          LoadCycle();
          break;
      case 1234:
          Cycles();
          break;                   
    } 
    
}


void InitializePort()
{
 while (GetCode != "1989")
 {
  GetCode = Serial.readString();
   if (GetCode == "1989")
   {
    Serial.println("1603");
    digitalWrite(InitializedPin, HIGH);
    delay(100);
    digitalWrite(InitializedPin, LOW);
    delay(100);
    digitalWrite(InitializedPin, HIGH);
    delay(100);
    digitalWrite(InitializedPin, LOW);
    delay(100);
    digitalWrite(InitializedPin, HIGH);
    delay(100);
    digitalWrite(InitializedPin, LOW);
    
   }
 }
}



int SerialReadInt()
{
  ReadCheck = false;
  
  while (!ReadCheck)
  if (Serial.available() > 1) 
  {                
        Serial.readBytes(Ati.array, 2);
        ReadCheck = true;         
       // digitalWrite(13, HIGH);
       // delay(500);
       // digitalWrite(13, LOW);
       // delay(500);  
                 
        return Ati.integer;              
  }        
}



void LoadCycle()
{
   
  CycleNumber = SerialReadInt();
 
  ItemsNumber = SerialReadInt();
  
  for (int i = 0; i<ItemsNumber; i++)
  {
    CycleItems[i].CycleDuration = SerialReadInt();
    CycleItems[i].OnTime = SerialReadInt();
    CycleItems[i].OffTime = SerialReadInt();
  } 
  
  SendData="1604";
//  for (int i = 0; i<ItemsNumber; i++)
//  {
 //   SendData+=(String)CycleItems[i].CycleDuration ;
  //  SendData+=(String)CycleItems[i].OnTime ;
//    SendData+=(String)CycleItems[i].OffTime ;
  //}
  
  Serial.println(SendData);
  CycleIsLoaded = true;
}



void Cycles()
{
  if (CycleIsLoaded)
  {
    for (int i = 0; i<CycleNumber; i++)
    {
      for (int k = 0; k<ItemsNumber; k++)
      {
        CycleTime = (unsigned long) CycleItems[k].CycleDuration*1000;
        CycleStart = millis();
        while ((millis() - CycleStart)<CycleTime &&(Serial.available()==0))
        {
         
          PulseTime = (unsigned long) CycleItems[k].OffTime*1000;
          PulseStart = millis();
          ShutterClose();
          while (((millis() - CycleStart)<CycleTime)&&((millis()-PulseStart)<PulseTime)&&(Serial.available()==0))
          {
            
          }
          
          PulseTime = (unsigned long) CycleItems[k].OnTime*1000;
          PulseStart = millis();
          ShutterOpen();
          while (((millis() - CycleStart)<CycleTime)&&((millis()-PulseStart)<PulseTime)&&(Serial.available()==0))
          {
            
          }
        }
      }
    }
  }
   
   Serial.println("1604");        
  ShutterClose();
}

void ShutterClose()
{
  if (!ShutterIsClosed)
  {
    ShutterIsClosed = !ShutterIsClosed;
     digitalWrite(ShatterOpenPin, LOW);
     StepUp(1);
  }
}

void ShutterOpen()
{
  if (ShutterIsClosed)
  {
    ShutterIsClosed = !ShutterIsClosed;
    digitalWrite(ShatterOpenPin, HIGH);
    StepDown(1);
  }
}

void Pulses(int PulseTime)
{
  int MPulstime = PulseTime*1000;
  while (Serial.available()==0)
  {
   digitalWrite(13, HIGH);
   delay(MPulstime);
   digitalWrite(13, LOW);
   delay(MPulstime);
  }
  Serial.println("1604");
}  

void StepUp(int Steps)
{
  for (int i=0; i<Steps; i++)
  {
    krok1();
   krok2();
    krok3();
   krok4();
    krok5();
     krok6();
  //  krok7();
  //  krok8();
  }
}


void StepDown(int Steps)
{
  for (int i=0; i<Steps; i++)
  {
 //  krok8();
  //  krok7();
   krok6();
   krok5();
  krok4();
  krok3();
   krok2();
   krok1();
  }
}


void StepDown1(int Steps)
{
  for (int i=0; i<Steps; i++)
  {
    digitalWrite(in1, LOW);
    digitalWrite(in2, LOW);
    digitalWrite(in3, LOW);
    digitalWrite(in4, HIGH);
    delay(rychlost);
    digitalWrite(in1, LOW);
    digitalWrite(in2, LOW);
    digitalWrite(in3, HIGH);
    digitalWrite(in4, LOW);
    delay(rychlost);
    digitalWrite(in1, LOW);
    digitalWrite(in2, HIGH);
    digitalWrite(in3, LOW);
    digitalWrite(in4, LOW);
    delay(rychlost);
    digitalWrite(in1, HIGH);
    digitalWrite(in2, LOW);
    digitalWrite(in3, LOW);
    digitalWrite(in4, LOW);
    delay(rychlost);   
  }
}
void StepUp1(int Steps)
{
  for (int i=0; i<Steps; i++)
  {
   digitalWrite(in1, HIGH);
    digitalWrite(in2, LOW);
    digitalWrite(in3, LOW);
    digitalWrite(in4, LOW);
    delay(rychlost);
    digitalWrite(in1, LOW);
    digitalWrite(in2, HIGH);
    digitalWrite(in3, LOW);
    digitalWrite(in4, LOW);
    delay(rychlost);
    digitalWrite(in1, LOW);
    digitalWrite(in2, LOW);
    digitalWrite(in3, HIGH);
    digitalWrite(in4, LOW);
    delay(rychlost);
    digitalWrite(in1, LOW);
    digitalWrite(in2, LOW);
    digitalWrite(in3, LOW);
    digitalWrite(in4, HIGH);
    delay(rychlost);
  }
}
void krok1()
  {    
    digitalWrite(in1, HIGH);
    digitalWrite(in2, LOW);
    digitalWrite(in3, LOW);
    digitalWrite(in4, LOW);
    delay(rychlost);
  }
void krok2()
  {
    digitalWrite(in1, HIGH);
    digitalWrite(in2, LOW);
    digitalWrite(in3, HIGH);
    digitalWrite(in4, LOW);
    delay(rychlost);
  }
void krok3()
  {
    digitalWrite(in1, LOW);
    digitalWrite(in2, HIGH);
    digitalWrite(in3, LOW);
    digitalWrite(in4, LOW);
    delay(rychlost);
  }
void krok4()
  {
    digitalWrite(in1, LOW);
    digitalWrite(in2, HIGH);
    digitalWrite(in3, HIGH);
    digitalWrite(in4, LOW);
    delay(rychlost);
  }
void krok5()
  {
    digitalWrite(in1, LOW);
    digitalWrite(in2, LOW);
    digitalWrite(in3, HIGH);
    digitalWrite(in4, LOW);
    delay(rychlost);
  }
void krok6()
  {
    digitalWrite(in1, LOW);
    digitalWrite(in2, HIGH);
    digitalWrite(in3, LOW);
    digitalWrite(in4, HIGH);
    delay(rychlost);
  }
void krok7()
  {
    digitalWrite(in1, LOW);
    digitalWrite(in2, LOW);
    digitalWrite(in3, LOW);
    digitalWrite(in4, HIGH);
    delay(rychlost);
  }
void krok8()
  {
    digitalWrite(in1, HIGH);
    digitalWrite(in2, LOW);
    digitalWrite(in3, LOW);
    digitalWrite(in4, HIGH);
    delay(rychlost);
  }
  
