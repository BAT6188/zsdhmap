#pragma once

#include <windows.h>

#ifdef __cplusplus
#   define HC_SDK_EXTERN_C extern "C"
#else
#   define HC_SDK_EXTERN_C
#endif

#if (defined WIN32 || defined _WIN32 || defined WINCE) && defined HC_SDK_API_EXPORTS
#   define HC_SDK_EXPORTS __declspec(dllexport)
#else
#   define HC_SDK_EXPORTS
#endif

#if defined WIN32 || defined _WIN32
#   define HC_SDK_CDECL __cdecl
#else
#   define HC_SDK_CDECL
#endif

#ifndef HC_SDK_API
#   define HC_SDK_API(rettype) HC_SDK_EXTERN_C HC_SDK_EXPORTS rettype HC_SDK_CDECL
#endif


typedef struct
{
    DWORD dwDisplayNo;//显示输出号
    BYTE  byDispChanType;/*输出连接模式,1-BNC，2-VGA，3-HDMI，4-DVI，5-SDI, 6-FIBER, 7-RGB, 8-YPrPb, 9-VGA/HDMI/DVI自适应，10-3GSDI,11-VGA/DVI自适应，0xff-无效*/
    BYTE  byRes[11];
}HC_SDK_DISPLAYPARAM, *LPHC_SDK_DISPLAYPARAM;

typedef struct
{
    DWORD  dwSize;
    HC_SDK_DISPLAYPARAM struDisplayParam[512];
    BYTE  byRes[128];
}HC_SDK_DISPLAYCFG, *LPHC_SDK_DISPLAYCFG;


typedef struct
{
    DWORD dwXCoordinate; /*矩形左上角起始点X坐标*/
    DWORD dwYCoordinate; /*矩形左上角Y坐标*/
    DWORD dwWidth;       /*矩形宽度*/
    DWORD dwHeight;      /*矩形高度*/
    BYTE  byRes[4];
}HC_SDK_RECTCFG, *LPHC_SDK_RECTCFG;

/*窗口信息*/
typedef struct
{
    DWORD dwSize;
    BYTE  byEnable;  //窗口使能,0-不使能，1-使能 
    BYTE  byRes1[7];
    DWORD dwWindowNo;//窗口号
    DWORD dwLayerIndex;//窗口相对应的图层号，图层号到最大即置顶，置顶操作
    HC_SDK_RECTCFG struRect;//目的窗口(相对显示墙)
    BYTE  byRes2[64];
}HC_SDK_VIDEOWALLWINDOWPOSITION, *LPHC_SDK_VIDEOWALLWINDOWPOSITION;

typedef struct
{
    DWORD dwWindowCount;//窗口号
    HC_SDK_VIDEOWALLWINDOWPOSITION struWinPos[512];
}HC_SDK_WALLWINDOWCFG, *LPHC_SDK_WALLWINDOWCFG;

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

    //初始化
    HC_SDK_API(BOOL)    HC_SDK_Init();

    //反初始化
    HC_SDK_API(BOOL)    HC_SDK_Uninit();

    //版本信息
    HC_SDK_API(BOOL)    HC_SDK_GetVersion(DWORD& dwSDKVer, DWORD& dwSDKBuildVer);

    //获取错误信息
    HC_SDK_API(DWORD)   HC_SDK_GetLastError();
    HC_SDK_API(char*)   HC_SDK_GetErrorInfo(DWORD dwErrorCode);

    //=============================B20====================================
    //登录B20：返回值用于后续设备操作
    HC_SDK_API(HANDLE)  HC_SDK_LoginB20(char *sDVRIP, WORD wDVRPort, char *sUserName, char *sPassword);
    //登出设备:
    HC_SDK_API(BOOL)    HC_SDK_Logout(HANDLE hHCSDK);

    //获取单位窗口的大小
    HC_SDK_API(BOOL)    HC_SDK_GetWindowBase(HANDLE hHCSDK, DWORD& dwWindowBaseX, DWORD& dwWindowBaseY);

    //==============显示通道操作接口==============
    //获取显示通道列表
    HC_SDK_API(BOOL)    HC_SDK_GetDisplayList(HANDLE hHCSDK, HC_SDK_DISPLAYCFG& tDisplayCfg);
    //获取显示输出口对应到电视墙上的位置。dwDisplayNo：显示通道号
    HC_SDK_API(BOOL)    HC_SDK_GetDisplayPosition(HANDLE hHCSDK, DWORD dwDisplayNo, HC_SDK_RECTCFG& tDisplayCfg);
    //将显示输出口对应到电视墙相应的位置上。byEnable:1(绑定)，0（取消绑定）
    HC_SDK_API(BOOL)    HC_SDK_SetDisplayPosition(HANDLE hHCSDK, DWORD dwDisplayNo, BYTE byEnable, const HC_SDK_RECTCFG& tDisplayCfg);

    //==============解码窗口操作接口==============
    //获取所有窗口
    HC_SDK_API(BOOL)    HC_SDK_GetDecodeWindow(HANDLE hHCSDK, HC_SDK_WALLWINDOWCFG& tWallWinCfg);
    //清空所有窗口
    HC_SDK_API(BOOL)    HC_SDK_ClearDecodeWindow(HANDLE hHCSDK);
    //打开解码窗口：返回值为窗口号，用户解码上墙的调用
    HC_SDK_API(DWORD)   HC_SDK_OpenDecodeWindow(HANDLE hHCSDK, const HC_SDK_RECTCFG& tWallWinCfg);
    //移动解码窗口
    HC_SDK_API(BOOL)    HC_SDK_MoveDecodeWindow(HANDLE hHCSDK, DWORD dwWindowNo, const HC_SDK_RECTCFG& tWallWinCfg);
    //关闭解码窗口
    HC_SDK_API(BOOL)    HC_SDK_CloseDecodeWindow(HANDLE hHCSDK, DWORD dwWindowNo);

    //==============解码上墙操作接口==============
    //启动解码上墙： dwWindowNo：解码窗口号，返回解码句柄用于送数据和停止解码接口
    HC_SDK_API(LONG)    HC_SDK_StartPassiveDecode(HANDLE hHCSDK, DWORD dwWindowNo);
    //发送解码数据
    HC_SDK_API(BOOL)    HC_SDK_SendPassiveData(HANDLE hHCSDK, LONG lPassiveHandle, char *pSendBuf, DWORD dwBufSize);
    //停止解码上墙
    HC_SDK_API(BOOL)    HC_SDK_StopPassiveDecode(HANDLE hHCSDK, LONG lPassiveHandle);

#ifdef __cplusplus
}
#endif /* __cplusplus */