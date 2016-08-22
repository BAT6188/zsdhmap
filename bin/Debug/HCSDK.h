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
    DWORD dwDisplayNo;//��ʾ�����
    BYTE  byDispChanType;/*�������ģʽ,1-BNC��2-VGA��3-HDMI��4-DVI��5-SDI, 6-FIBER, 7-RGB, 8-YPrPb, 9-VGA/HDMI/DVI����Ӧ��10-3GSDI,11-VGA/DVI����Ӧ��0xff-��Ч*/
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
    DWORD dwXCoordinate; /*�������Ͻ���ʼ��X����*/
    DWORD dwYCoordinate; /*�������Ͻ�Y����*/
    DWORD dwWidth;       /*���ο��*/
    DWORD dwHeight;      /*���θ߶�*/
    BYTE  byRes[4];
}HC_SDK_RECTCFG, *LPHC_SDK_RECTCFG;

/*������Ϣ*/
typedef struct
{
    DWORD dwSize;
    BYTE  byEnable;  //����ʹ��,0-��ʹ�ܣ�1-ʹ�� 
    BYTE  byRes1[7];
    DWORD dwWindowNo;//���ں�
    DWORD dwLayerIndex;//�������Ӧ��ͼ��ţ�ͼ��ŵ�����ö����ö�����
    HC_SDK_RECTCFG struRect;//Ŀ�Ĵ���(�����ʾǽ)
    BYTE  byRes2[64];
}HC_SDK_VIDEOWALLWINDOWPOSITION, *LPHC_SDK_VIDEOWALLWINDOWPOSITION;

typedef struct
{
    DWORD dwWindowCount;//���ں�
    HC_SDK_VIDEOWALLWINDOWPOSITION struWinPos[512];
}HC_SDK_WALLWINDOWCFG, *LPHC_SDK_WALLWINDOWCFG;

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

    //��ʼ��
    HC_SDK_API(BOOL)    HC_SDK_Init();

    //����ʼ��
    HC_SDK_API(BOOL)    HC_SDK_Uninit();

    //�汾��Ϣ
    HC_SDK_API(BOOL)    HC_SDK_GetVersion(DWORD& dwSDKVer, DWORD& dwSDKBuildVer);

    //��ȡ������Ϣ
    HC_SDK_API(DWORD)   HC_SDK_GetLastError();
    HC_SDK_API(char*)   HC_SDK_GetErrorInfo(DWORD dwErrorCode);

    //=============================B20====================================
    //��¼B20������ֵ���ں����豸����
    HC_SDK_API(HANDLE)  HC_SDK_LoginB20(char *sDVRIP, WORD wDVRPort, char *sUserName, char *sPassword);
    //�ǳ��豸:
    HC_SDK_API(BOOL)    HC_SDK_Logout(HANDLE hHCSDK);

    //��ȡ��λ���ڵĴ�С
    HC_SDK_API(BOOL)    HC_SDK_GetWindowBase(HANDLE hHCSDK, DWORD& dwWindowBaseX, DWORD& dwWindowBaseY);

    //==============��ʾͨ�������ӿ�==============
    //��ȡ��ʾͨ���б�
    HC_SDK_API(BOOL)    HC_SDK_GetDisplayList(HANDLE hHCSDK, HC_SDK_DISPLAYCFG& tDisplayCfg);
    //��ȡ��ʾ����ڶ�Ӧ������ǽ�ϵ�λ�á�dwDisplayNo����ʾͨ����
    HC_SDK_API(BOOL)    HC_SDK_GetDisplayPosition(HANDLE hHCSDK, DWORD dwDisplayNo, HC_SDK_RECTCFG& tDisplayCfg);
    //����ʾ����ڶ�Ӧ������ǽ��Ӧ��λ���ϡ�byEnable:1(��)��0��ȡ���󶨣�
    HC_SDK_API(BOOL)    HC_SDK_SetDisplayPosition(HANDLE hHCSDK, DWORD dwDisplayNo, BYTE byEnable, const HC_SDK_RECTCFG& tDisplayCfg);

    //==============���봰�ڲ����ӿ�==============
    //��ȡ���д���
    HC_SDK_API(BOOL)    HC_SDK_GetDecodeWindow(HANDLE hHCSDK, HC_SDK_WALLWINDOWCFG& tWallWinCfg);
    //������д���
    HC_SDK_API(BOOL)    HC_SDK_ClearDecodeWindow(HANDLE hHCSDK);
    //�򿪽��봰�ڣ�����ֵΪ���ںţ��û�������ǽ�ĵ���
    HC_SDK_API(DWORD)   HC_SDK_OpenDecodeWindow(HANDLE hHCSDK, const HC_SDK_RECTCFG& tWallWinCfg);
    //�ƶ����봰��
    HC_SDK_API(BOOL)    HC_SDK_MoveDecodeWindow(HANDLE hHCSDK, DWORD dwWindowNo, const HC_SDK_RECTCFG& tWallWinCfg);
    //�رս��봰��
    HC_SDK_API(BOOL)    HC_SDK_CloseDecodeWindow(HANDLE hHCSDK, DWORD dwWindowNo);

    //==============������ǽ�����ӿ�==============
    //����������ǽ�� dwWindowNo�����봰�ںţ����ؽ��������������ݺ�ֹͣ����ӿ�
    HC_SDK_API(LONG)    HC_SDK_StartPassiveDecode(HANDLE hHCSDK, DWORD dwWindowNo);
    //���ͽ�������
    HC_SDK_API(BOOL)    HC_SDK_SendPassiveData(HANDLE hHCSDK, LONG lPassiveHandle, char *pSendBuf, DWORD dwBufSize);
    //ֹͣ������ǽ
    HC_SDK_API(BOOL)    HC_SDK_StopPassiveDecode(HANDLE hHCSDK, LONG lPassiveHandle);

#ifdef __cplusplus
}
#endif /* __cplusplus */