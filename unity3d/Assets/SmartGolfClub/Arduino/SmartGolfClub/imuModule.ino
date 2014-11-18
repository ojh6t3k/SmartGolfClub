//******************************************************************************
//* Includes
//******************************************************************************
#include "UnityRobot.h"
#include "imuModule.h"


//******************************************************************************
//* Constructors
//******************************************************************************

imuModule::imuModule(int id) : UnityModule(id, false)
{
	_qX = 0;
    _qY = 0;
    _qZ = 0;
	_qW = 0;
}

void imuModule::SetQuarternion(float x, float y, float z, float w)
{
	_qX = (short)(x * 100);
    _qY = (short)(y * 100);
    _qZ = (short)(z * 100);
	_qW = (short)(w * 100);
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void imuModule::OnSetup()
{
}

void imuModule::OnStart()
{
	_preTime = millis();
}

void imuModule::OnStop()
{	
}

void imuModule::OnProcess()
{	
}

void imuModule::OnUpdate()
{
}

void imuModule::OnAction()
{
}

void imuModule::OnFlush()
{
	unsigned long curTime = millis();
	_intervalTime = (word)(curTime - _preTime);
	_preTime = curTime;

	UnityRobot.push(_qX);
    UnityRobot.push(_qY);
    UnityRobot.push(_qZ);
	UnityRobot.push(_qW);
	UnityRobot.push(_intervalTime);
}
