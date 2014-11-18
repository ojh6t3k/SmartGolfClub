#ifndef imuModule_h
#define imuModule_h

#include "UnityModule.h"


class imuModule : UnityModule
{
public:
	imuModule(int id);

    void SetQuarternion(float x, float y, float z, float w);

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnAction();
	void OnFlush();

private:
    short _qX;
    short _qY;
    short _qZ;
	short _qW;
	word _intervalTime;
	unsigned long _preTime;
};

#endif

