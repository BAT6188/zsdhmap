#pragma once

#include <WinSock2.h>
#include "gecommon.h"

#ifdef __cplusplus
#   define NVIEW_SDK_EXTERN_C extern "C"
#else
#   define NVIEW_SDK_EXTERN_C
#endif

#if (defined WIN32 || defined _WIN32 || defined WINCE) && defined NVIEW_SDK_API_EXPORTS
#   define NVIEW_SDK_EXPORTS __declspec(dllexport)
#else
#   define NVIEW_SDK_EXPORTS
#endif

#if defined WIN32 || defined _WIN32
#   define NVIEW_SDK_CDECL __cdecl
#else
#   define NVIEW_SDK_CDECL
#endif

#ifndef NVIEW_SDK_API
#   define NVIEW_SDK_API(rettype) NVIEW_SDK_EXTERN_C NVIEW_SDK_EXPORTS rettype NVIEW_SDK_CDECL
#endif

#define MSG_TYPE_LOGIN						1
#define MSG_TYPE_LOGOUT						2
#define MSG_TYPE_START_MONITOR				3
#define MSG_TYPE_STOP_MONITOR				4
#define MSG_TYPE_PTZ_CONTROL				5
#define MSG_TYPE_QUERY_RECORD				6
#define MSG_TYPE_START_PLAYBACK				7
#define MSG_TYPE_STOP_PLAYBACK				8
#define MSG_TYPE_CTRL_PLAYBACK				9
#define MSG_TYPE_DOWNLOAD_RECORD			10
#define MSG_TYPE_START_AUDIO				11 //���������Խ���Ϣ
#define MSG_TYPE_STOP_AUDIO					MSG_TYPE_STOP_MONITOR //ֹͣ�����Խ���Ϣ

#define NOTIFY_TYPE_ALARM					100
#define NOTIFY_TYPE_FD_ONLINE				101
#define NOTIFY_TYPE_DOWNLOAD_URL			102

//net type
#define NET_TYPE_UDP				1
#define NET_TYPE_TCP				2

enum EM_PLAYBACK_CTRL_ACTION
{
    EM_PLAYBACK_CTRL_ACTION_FastForward = 0x01,//���
    EM_PLAYBACK_CTRL_ACTION_FastBackward = 0x02,//����
    EM_PLAYBACK_CTRL_ACTION_Pause = 0x03,//��ͣ 
    EM_PLAYBACK_CTRL_ACTION_Stop = 0x04,//ֹͣ 
    EM_PLAYBACK_CTRL_ACTION_Continue = 0x05,//�����ط� 
    EM_PLAYBACK_CTRL_ACTION_DragPlay = 0x06,//�ط��϶�
    EM_PLAYBACK_CTRL_ACTION_SlowForward = 0x07,//���� 
    EM_PLAYBACK_CTRL_ACTION_SlowBackward = 0x08,//����
    EM_PLAYBACK_CTRL_ACTION_FramePlay = 0x09,//��֡��
    EM_PLAYBACK_CTRL_ACTION_Forward = 0x0a,//�Ե�ǰ������ǰ��
    EM_PLAYBACK_CTRL_ACTION_Backward = 0x0b,//�Ե�ǰ��������
};

enum EM_CAMERA_CONTROL_ACTION
{
    EM_CAMERA_CONTROL_ACTION_UP = 0x00000001 /*����ת*/,
    EM_CAMERA_CONTROL_ACTION_LEFT = 0x00000002 /*����ת*/,
    EM_CAMERA_CONTROL_ACTION_ROTATE = 0x00000003 /*��ת*/,
    EM_CAMERA_CONTROL_ACTION_RIGHT = 0x00000004 /*����ת*/,
    EM_CAMERA_CONTROL_ACTION_DOWN = 0x00000005 /*����ת*/,
    EM_CAMERA_CONTROL_ACTION_FAR = 0x00000006 /*��ͷ��Զ*/,
    EM_CAMERA_CONTROL_ACTION_FOCUSNEAR = 0x00000007 /*�۽�*/,
    EM_CAMERA_CONTROL_ACTION_AUTO = 0x00000008 /*�Զ�*/,
    EM_CAMERA_CONTROL_ACTION_FOCUSFAR = 0x00000009 /*ɢ��*/,
    EM_CAMERA_CONTROL_ACTION_NEAR = 0x0000000a /*��ͷ����*/,
    EM_CAMERA_CONTROL_ACTION_DIAPHRAGM_LARGE = 0x0000000b /*��Ȧ���󣨱�����*/,
    EM_CAMERA_CONTROL_ACTION_DIAPHRAGM_SMALL = 0x0000000c /*��Ȧ��С���䰵��*/,
    EM_CAMERA_CONTROL_ACTION_STOP = 0x0000000d /*ֹͣ����*/,
    EM_CAMERA_CONTROL_ACTION_SPEAKER_ON = 0x0000000e /*������*/,
    EM_CAMERA_CONTROL_ACTION_LIGHT_ON = 0x0000000f /*�򿪵ƹ�*/,
    EM_CAMERA_CONTROL_ACTION_HOTDOG = 0x00000010 /*�ȹ�ɨ��*/,
    EM_CAMERA_CONTROL_ACTION_SPEAKER_OFF = 0x00000011 /*�ر�����*/,
    EM_CAMERA_CONTROL_ACTION_LIGHT_OFF = 0x00000012 /*�رյƹ�*/,
    EM_CAMERA_CONTROL_ACTION_PRESET_GOTO = 0x00000013 /*�л���Ԥ�õ�*/,
    EM_CAMERA_CONTROL_ACTION_PRESET_SET = 0x00000014 /*����Ԥ�õ�*/,
    EM_CAMERA_CONTROL_ACTION_PRESET_DEL = 0x00000015 /*ɾ��Ԥ�õ�*/,
    EM_CAMERA_CONTROL_ACTION_CAMERA_RESET = 0x00000016 /*�������λ*/,
    EM_CAMERA_CONTROL_ACTION_WIPER_ON = 0x00000017 /*����ˢ*/,
    EM_CAMERA_CONTROL_ACTION_WIPER_OFF = 0x00000018 /*�ر���ˢ*/,
    EM_CAMERA_CONTROL_ACTION_AUTOCRUISE = 0x0000001d /*�Զ�Ѳ��*/,
    EM_CAMERA_CONTROL_ACTION_PRESET_CLEAR = 0x0000001e /*�������Ԥ�õ�*/,
    EM_CAMERA_CONTROL_ACTION_STARTTRACKING = 0x0000001f /*��������*/,
    EM_CAMERA_CONTROL_ACTION_STOPTRACKING = 0x00000020 /*ֹͣ����*/,
    EM_CAMERA_CONTROL_ACTION_LEFTUP = 0x00000021 /*����ת*/,
    EM_CAMERA_CONTROL_ACTION_RIGHTUP = 0x00000022 /*����ת*/,
    EM_CAMERA_CONTROL_ACTION_LEFTDOWN = 0x00000023 /*����ת*/,
    EM_CAMERA_CONTROL_ACTION_RIGHTDOWN = 0x00000024 /*����ת*/,
    EM_CAMERA_CONTROL_ACTION_CAMERAACTIVE = 0x00000028 /*���������*/,
    EM_CAMERA_CONTROL_ACTION_SETPANSPEED = 0x00000029 /*��������ת���ٶ�*/,
    EM_CAMERA_CONTROL_ACTION_SETTILTSPEED = 0x00000030 /*��������ת���ٶ�*/,
    EM_CAMERA_CONTROL_ACTION_SETZOOMSPEED = 0x00000031 /*���ù�Ȧ�ٶ�*/,
    EM_CAMERA_CONTROL_ACTION_SETFOCUSSPEED = 0x00000032 /*���þ۽�/ɢ���ٶ�*/,
    EM_CAMERA_CONTROL_ACTION_SPEEDSETTINGGET = 0x00000033 /*��ȡ������ٶ�����*/,
    EM_CAMERA_CONTROL_ACTION_MATRIXSWITCH = 0x00000040 /*�����л�*/,
    EM_CAMERA_CONTROL_ACTION_BRIGHTNESS = 0x00000050 /*����*/,
    EM_CAMERA_CONTROL_ACTION_CONTRAST = 0x00000051 /*�Աȶ�*/,
    EM_CAMERA_CONTROL_ACTION_SATURATION = 0x00000052 /*���Ͷ�*/,
    EM_CAMERA_CONTROL_ACTION_HUE = 0x00000053 /*ɫ��*/,
};

enum NVIEW_SDK_ERRORCODE
{
    NVIEW_SDK_OK = 0x00000000,
    NVIEW_SDK_SUCCESS_COMMON_BASE = 0x00010000 /*UAS BASE*/,
    NVIEW_SDK_SUCCESS_UA_BASE = 0x00020000,
    NVIEW_SDK_SUCCESS_UA_UPDATE = 0x00020001,
    NVIEW_SDK_SUCCESS_UA_REDIRECT = 0x00020002,
    NVIEW_SDK_SUCCESS_UA_LOGINREPLACED = 0x00020003,
    NVIEW_SDK_SUCCESS_FD_BASE = 0x00030000,
    NVIEW_SDK_SUCCESS_AAA_BASE = 0x00040000,
    NVIEW_SDK_SUCCESS_MSS_BASE = 0x00050000,
    NVIEW_SDK_SUCCESS_DBMS_BASE = 0x00060000,
    NVIEW_SDK_SUCCESS_FDMS_BASE = 0x00070000,
    NVIEW_SDK_SUCCESS_CMS_BASE = 0x00080000,
    NVIEW_SDK_ERROR_COMMON_BASE = 0x80010000,
    NVIEW_SDK_ERROR_COMMON_UNPACK = 0x80010001,
    NVIEW_SDK_ERROR_COMMON_PACK = 0x80010002,
    NVIEW_SDK_ERROR_COMMON_SERVICE_UNAVAILABLE = 0x80010003,
    NVIEW_SDK_ERROR_COMMON_CMDERROR = 0x80010004,
    NVIEW_SDK_ERROR_COMMON_NODBMSPROXY = 0x80010005,
    NVIEW_SDK_ERROR_COMMON_NOFDMSPROXY = 0x80010006,
    NVIEW_SDK_ERROR_COMMON_NOMSSPROXY = 0x80010007,
    NVIEW_SDK_ERROR_COMMON_DBMSFAIL = 0x80010008,
    NVIEW_SDK_ERROR_COMMON_DBMS_NORECORD = 0x80010009,
    NVIEW_SDK_ERROR_COMMON_MSSFAIL = 0x8001000a,
    NVIEW_SDK_ERROR_COMMON_FDMSFAIL = 0x8001000b,
    NVIEW_SDK_ERROR_COMMON_ICEEXCEPTION = 0x8001000c,
    NVIEW_SDK_ERROR_COMMON_RESULTERROR = 0x8001000d,
    NVIEW_SDK_ERROR_COMMON_MONITOR_IN_PROCESS = 0x8001000e,
    NVIEW_SDK_ERROR_COMMON_MONITOR_NOT_IN_PROCESS = 0x8001000f,
    NVIEW_SDK_ERROR_COMMON_RECORD_IN_PROCESS = 0x80010010,
    NVIEW_SDK_ERROR_COMMON_RECORD_NOT_IN_PROCESS = 0x80010011,
    NVIEW_SDK_ERROR_COMMON_DOWNLOAD_IN_PROCESS = 0x80010012,
    NVIEW_SDK_ERROR_COMMON_DOWNLOAD_NOT_IN_PROCESS = 0x80010013,
    NVIEW_SDK_ERROR_COMMON_PLAYBACK_IN_PROCESS = 0x80010014,
    NVIEW_SDK_ERROR_COMMON_PLAYBACK_NOT_IN_PROCESS = 0x80010015,
    NVIEW_SDK_ERROR_COMMON_USER_CANCELED = 0x80010016 /*�����ѱ��û�ȡ�������緢��STOP*/,
    NVIEW_SDK_ERROR_COMMON_NOTSUPPORT = 0x80010017 /*���ܲ�֧��*/,
    NVIEW_SDK_ERROR_COMMON_RECORD_TIME = 0x80010018,
    NVIEW_SDK_ERROR_COMMON_REACH_MAX = 0x80010019,
    NVIEW_SDK_ERROR_COMMON_CSS_NOSPACE = 0x8001001a /*���Ĵ洢���ϵ��ܿռ�Ϊ0*/,
    NVIEW_SDK_ERROR_COMMON_MSS_INVALID_PARAM = 0x8001001b,
    NVIEW_SDK_ERROR_COMMON_REACH_CUSTOMERMAX = 0x8001001c,
    NVIEW_SDK_ERROR_COMMON_TIME = 0x8001001d /*ʱ�������ץ�ġ�¼�����Ҫָ��ʱ���ָ���У�ʱ����Ч*/,
    NVIEW_SDK_ERROR_COMMON_LOCKED = 0x8001001e /*�豸����ס������̨���������У���̨�������û���ס*/,
    NVIEW_SDK_ERROR_COMMON_REMOTE_DOMAIN_OFFLINE = 0x8001001f /*���������δ����*/,
    NVIEW_SDK_ERROR_COMMON_VERSION_TOOLD = 0x80010020 /*�汾̫��*/,
    NVIEW_SDK_ERROR_COMMON_BUSY = 0x80010021 /*̫æ*/,
    NVIEW_SDK_ERROR_UA_BASE = 0x80020000,
    NVIEW_SDK_ERROR_UA_NOTEXIST = 0x80020001,
    NVIEW_SDK_ERROR_UA_OUTOFDATE = 0x80020002,
    NVIEW_SDK_ERROR_UA_LOGINFAIL = 0x80020003,
    NVIEW_SDK_ERROR_UA_NOPRIVILEGE = 0x80020004,
    NVIEW_SDK_ERROR_UA_ERRORTYPE = 0x80020005,
    NVIEW_SDK_ERROR_UA_EMAILEXISTED = 0x80020006,
    NVIEW_SDK_ERROR_UA_CMDNOCONTENT = 0x80020007,
    NVIEW_SDK_ERROR_UA_NOTSPECIFIED = 0x80020008,
    NVIEW_SDK_ERROR_UA_UACANCELED = 0x80020009,
    NVIEW_SDK_ERROR_UA_CLIENTCANCELED = 0x8002000a,
    NVIEW_SDK_ERROR_FD_BASE = 0x80030000,
    NVIEW_SDK_ERROR_AAA_BASE = 0x80040000,
    NVIEW_SDK_ERROR_AAA_UNKNOWN = 0x80040001 /*δ֪ԭ�����*/,
    NVIEW_SDK_ERROR_AAA_INVALIDPACKET = 0x80040002 /*Radius�����Ϸ�*/,
    NVIEW_SDK_ERROR_AAA_MISSATTRIBUTE = 0x80040003 /*Radius��ȱ�ٱ�Ҫ������*/,
    NVIEW_SDK_ERROR_AAA_INTERNAL = 0x80040004 /*AAA�����������ڲ�����*/,
    NVIEW_SDK_ERROR_AAA_DBMS = 0x80040005 /*AAA�������������ݿ�ʧ��*/,
    NVIEW_SDK_ERROR_AAA_STOPPED = 0x80040006 /*AAA������ֹͣ����*/,
    NVIEW_SDK_ERROR_AAA_PASSWORD = 0x80040007 /*�������*/,
    NVIEW_SDK_ERROR_AAA_BADUSERID = 0x80040008 /*�û�ID������*/,
    NVIEW_SDK_ERROR_AAA_BADDEVID = 0x80040009 /*�豸ID������*/,
    NVIEW_SDK_ERROR_AAA_BADCHANNEL = 0x8004000a /*�豸ͨ���Ų�����*/,
    NVIEW_SDK_ERROR_AAA_FORBIDDEN = 0x8004000b /*û��Ȩ��ִ��������Ĳ���*/,
    NVIEW_SDK_ERROR_AAA_NOMONEY = 0x8004000c /*���ò���*/,
    NVIEW_SDK_ERROR_AAA_NODISK = 0x8004000d /*��������*/,
    NVIEW_SDK_ERROR_AAA_CUSTOMERSTATUS = 0x8004000e /*�ͻ�״̬������*/,
    NVIEW_SDK_ERROR_AAA_USERSTATUS = 0x8004000f /*�û�״̬������*/,
    NVIEW_SDK_ERROR_AAA_USERIPDENIED = 0x80040010 /*�û�IP��ַ�ܾ�*/,
    NVIEW_SDK_ERROR_AAA_SMSCODEFAIL = 0x80040011 /*���ŵ�¼��������*/,
    NVIEW_SDK_ERROR_AAA_SMSBAD = 0x80040012 /*������֤�����*/,
    NVIEW_SDK_ERROR_AAA_SMSSENDFAILED = 0x80040013 /*������֤�뷢��ʧ��*/,
    NVIEW_SDK_ERROR_MSS_BASE = 0x80050000,
    NVIEW_SDK_ERROR_MSS_FAIL = 0x80050001 /*:-1         ʧ��*/,
    NVIEW_SDK_ERROR_MSS_INVALID_ID = 0x80050002 /*:-2         �Ƿ�ID*/,
    NVIEW_SDK_ERROR_MSS_INVALID_HANDLE = 0x80050003 /*:-3         �Ƿ����*/,
    NVIEW_SDK_ERROR_MSS_INVALID_PARAM = 0x80050004 /*:-4         �Ƿ�����*/,
    NVIEW_SDK_ERROR_MSS_NULLPTR = 0x80050005 /*:-5         ָ��ΪNULL*/,
    NVIEW_SDK_ERROR_MSS_REPEAT = 0x80050006 /*:-6         �ظ�����*/,
    NVIEW_SDK_ERROR_MSS_TIMEOUT = 0x80050007 /*:-7         ��ʱ*/,
    NVIEW_SDK_ERROR_MSS_OUTOF_MEMORY = 0x80050008 /*:-8         �ڴ治��*/,
    NVIEW_SDK_ERROR_MSS_OUTOF_RANGE = 0x80050009 /*:-9         ������Χ*/,
    NVIEW_SDK_ERROR_MSS_FULL = 0x8005000a /*:-10        ��������*/,
    NVIEW_SDK_ERROR_MSS_UNEXPECTED = 0x8005000b /*:-11        δ֪�쳣*/,
    NVIEW_SDK_ERROR_MSS_PRIVILEGE_LIMIT = 0x8005000c /*:-12        Ȩ������*/,
    NVIEW_SDK_ERROR_MSS_BUSY = 0x80050100 /*MSS����æ*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_FD = 0x80050101 /*MSS����FDʧ��*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_RTMDS = 0x80050102 /*MSS����RTMDSʧ��*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_SMS = 0x80050103 /*MSS����SMSʧ��*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_FTS = 0x80050104 /*MSS����FTSʧ��*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_CSS = 0x80050105 /*MSS����CSSʧ��*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_CANCELLED = 0x80050106 /*MSS���ȱ�ȡ��*/,
    NVIEW_SDK_ERROR_MSS_FD_OFFLINE = 0x80050200 /*FDδ����, ����VPN��*/,
    NVIEW_SDK_ERROR_MSS_FD_CHANNEL_INVALID = 0x80050201 /*FDδ����*/,
    NVIEW_SDK_ERROR_MSS_FD_BUSY = 0x80050202 /*FDæ*/,
    NVIEW_SDK_ERROR_MSS_FD_NO_RIGHT = 0x80050203 /*FD����δ����Ȩ*/,
    NVIEW_SDK_ERROR_MSS_FD_FAILTURE = 0x80050204 /*FD����ʧ��*/,
    NVIEW_SDK_ERROR_MSS_FD_RETURN_ERROR = 0x80050205 /*FD���ش���*/,
    NVIEW_SDK_ERROR_MSS_FD_PACKAGE_ERROR = 0x80050206 /*FD�������*/,
    NVIEW_SDK_ERROR_MSS_FDMS_OFFLINE = 0x80050300 /*FDMS����δ���� 0x300*/,
    NVIEW_SDK_ERROR_MSS_FDMS_TIMEOUT = 0x80050301 /*FDMS������Ӧ��ʱ*/,
    NVIEW_SDK_ERROR_MSS_FDMS_FAILTURE = 0x80050302 /*FDMS������Ӧʧ��*/,
    NVIEW_SDK_ERROR_MSS_RTMDS_OFFLINE = 0x80050400 /*RTMDS����δ���� 0x400*/,
    NVIEW_SDK_ERROR_MSS_RTMDS_TIMEOUT = 0x80050401 /*RTMDS������Ӧ��ʱ*/,
    NVIEW_SDK_ERROR_MSS_RTMDS_FAILTURE = 0x80050402 /*RTMDS������Ӧʧ��*/,
    NVIEW_SDK_ERROR_MSS_SMS_OFFLINE = 0x80050500 /*SMS����δ����  0x500*/,
    NVIEW_SDK_ERROR_MSS_SMS_TIMEOUT = 0x80050501 /*SMS������Ӧ��ʱ*/,
    NVIEW_SDK_ERROR_MSS_SMS_FAILTURE = 0x80050502 /*SMS������Ӧʧ��*/,
    NVIEW_SDK_ERROR_MSS_SMS_NO_FILE = 0x80050503 /*SMS������Ӧ�ļ�������*/,
    NVIEW_SDK_ERROR_MSS_FTS_OFFLINE = 0x80050600 /*FTS����δ���� 0x600*/,
    NVIEW_SDK_ERROR_MSS_FTS_TIMEOUT = 0x80050601 /*FTS������Ӧ��ʱ*/,
    NVIEW_SDK_ERROR_MSS_FTS_FAILTURE = 0x80050602 /*FTS������Ӧʧ��*/,
    NVIEW_SDK_ERROR_MSS_FTS_NO_FILE = 0x80050603 /*FTS������Ӧ�ļ�������*/,
    NVIEW_SDK_ERROR_MSS_CSS_OFFLINE = 0x80050700 /*CSS����δ���� 0x700*/,
    NVIEW_SDK_ERROR_MSS_CSS_TIMEOUT = 0x80050701 /*CSS������Ӧ��ʱ*/,
    NVIEW_SDK_ERROR_MSS_CSS_FAILTURE = 0x80050702 /*CSS������Ӧʧ��*/,
    NVIEW_SDK_ERROR_MSS_CSS_NO_FILE = 0x80050703 /*CSS������Ӧ�ļ�������*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORDING = 0x80050704 /*CSS������Ӧ����¼��*/,
    NVIEW_SDK_ERROR_MSS_CSS_BUSY = 0x80050705 /*CSS������Ӧæ*/,
    NVIEW_SDK_ERROR_MSS_CSS_DISK_FULL = 0x80050706 /*CSS������Ӧ��������*/,
    NVIEW_SDK_ERROR_MSS_CSS_DISK_QUOTA = 0x80050707 /*CSS������Ӧ�����޶�*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_STOP = 0x80050708 /*CSS���񱨸�¼��ֹͣ*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_EXPIRE = 0x80050709 /*CSS���񱨸�¼��ʱ�䵽��*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_QUOTA = 0x8005070a /*CSS���񱨸�¼������޶���*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_NETWORKIO = 0x8005070b /*CSS���񱨸�¼������IO����*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_FILEIO = 0x8005070c /*CSS���񱨸�¼���ļ�IO����*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_STOPBYCMS = 0x8005070d /*CSS���񱨸�¼��ϵͳ����Աֹͣ*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_NOPACKET = 0x8005070e /*CSS���񱨸�¼����һ�������ڽ��յ������ݰ�̫��*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_NOFRAME = 0x8005070f /*CSS���񱨸�¼����һ�������ڽ��յ������ݰ�����ɵ�֡��Ϊ0*/,
    NVIEW_SDK_ERROR_MSS_SSU_OFFLINE = 0x80050d00,
    NVIEW_SDK_ERROR_MSS_SSU_TIMEOUT = 0x80050d01,
    NVIEW_SDK_ERROR_MSS_SSU_FAILTURE = 0x80050d02,
    NVIEW_SDK_ERROR_MSS_SID_MISMATCH = 0x80059000 /*��������sessionid��ƥ��*/,
    NVIEW_SDK_ERROR_DBMS_BASE = 0x80060000,
    NVIEW_SDK_ERROR_DBMS_QUERY_FD_DOMAIN = 0x80060001,
    NVIEW_SDK_ERROR_DBMS_CUSTOM_NOTEXISTS = 0x80060002 /*DBMS��ѯ�ͻ�������*/,
    NVIEW_SDK_ERROR_FDMS_BASE = 0x80070000 /*generic error*/,
    NVIEW_SDK_ERROR_FDMS_DBMS = 0x80075000 /*fdms-dbms error*/,
    NVIEW_SDK_ERROR_FDMS_FD_BASE = 0x80077000 /*fd generic error*/,
    NVIEW_SDK_ERROR_FDMS_FD_TOO_MANY = 0x80077001 /*too many fd                                    1*/,
    NVIEW_SDK_ERROR_FDMS_FD_BAD_STATE = 0x80077002 /*fd bad state, disabled in dbms                2*/,
    NVIEW_SDK_ERROR_FDMS_FD_LICENSE = 0x80077003 /*license, too many fd                            3*/,
    NVIEW_SDK_ERROR_FDMS_FD_MSG_SENDFAILED = 0x80077004 /*failed send msg to fd                    4*/,
    NVIEW_SDK_ERROR_FDMS_FD_MULTI_LOGIN = 0x80077005 /*login more than 1 time                      5*/,
    NVIEW_SDK_ERROR_FDMS_FD_CANCELED = 0x80077006 /*canceled                                       6*/,
    NVIEW_SDK_ERROR_FDMS_FD_NOT_IN_DB = 0x80077007 /*fd not in the database                        7*/,
    NVIEW_SDK_ERROR_FDMS_FD_EXCEPTION = 0x80077008 /*fd response exception, such as unpack failed  8*/,
    NVIEW_SDK_ERROR_FDMS_FD_BAD_FDID = 0x80077009 /*bad fdid, such as length != 18                 9*/,
    NVIEW_SDK_ERROR_FDMS_FD_TIMEOUT = 0x8007700a /*fd response timeout                             a*/,
    NVIEW_SDK_ERROR_FDMS_FD_NOT_EXIST = 0x8007700b /*fd not online                                 b*/,
    NVIEW_SDK_ERROR_FDMS_FD_FACTORY = 0x8007700c /*fd factory license, too many fd                 c*/,
    NVIEW_SDK_ERROR_FDMS_FD_PTZLOCKED = 0x8007700d /*ptz locked                                    d*/,
    NVIEW_SDK_ERROR_FDMS_BUSY = 0x8007700e /*too many fd logining, please wait                     e*/,
    NVIEW_SDK_ERROR_FDMS_FD_NOTSUPPORTED = 0x8007700f /*not supported by fd                        f*/,
    NVIEW_SDK_ERROR_FDMS_AAA = 0x80078000,
    NVIEW_SDK_ERROR_FDMS_AAA_PARAM = 0x80078001 /*fd login failed because of bad parameter*/,
    NVIEW_SDK_ERROR_FDMS_AAA_AUTH = 0x80078002 /*fd login failed because of bad password*/,
    NVIEW_SDK_ERROR_CMS_BASE = 0x80080000,
    NVIEW_SDK_ERROR_CMS_NAMEID_INDEX_OVERFLOW = 0x80080001 /*�±���Խ��*/,
    NVIEW_SDK_ERROR_CMS_ID_NOTEXIST = 0x80080002 /*���󲻴���*/,
    NVIEW_SDK_ERROR_CMS_TRAP_NOTSETABLE = 0x80080003 /*���󲻿�����*/,
    NVIEW_SDK_ERROR_CMS_CLASS_NOTEXIST = 0x80080004 /*�಻����*/,
    NVIEW_SDK_ERROR_CMS_METHOD_NOTEXIST = 0x80080005 /*����������*/,
    NVIEW_SDK_ERROR_CMS_MEMBER_NOTEXIST = 0x80080006 /*��Ա������*/,
    NVIEW_SDK_ERROR_PARALLEL_INTERVAL = 0x10000000,
    NVIEW_SDK_ERROR_PARALLEL_BASE = 0x90090000,
    NVIEW_SDK_ERROR_PARALLEL_TIMEOUT = 0x90090001,
    NVIEW_SDK_ERROR_PARALLEL_DOMAINNOTEXIST = 0x90090002,
    NVIEW_SDK_ERROR_PARALLEL_XMLPACK = 0x90090003,
    NVIEW_SDK_ERROR_PARALLEL_XMLUNPACK = 0x90090004,
    NVIEW_SDK_ERROR_PARALLEL_UPACKFDMSRES = 0x90090005,
};

typedef struct
{
    char            channel_name[128]/*ͨ������*/;
    char            channel_code[36]/*ͨ����ʽ��200000-200000000100011835-0001-0001*/;
    char            db33_code[19]/*DB33���*/;
    char            gab_code[21]/*������*/;
    unsigned char   isonline/*�豸����״̬*/;
    unsigned short  factory_code/*���̴��룬�鿴��Ƶ��ʱ��Ͳ����ٲ���ϸ��Ϣ��*/;
    unsigned int    channel_status/*BIT 1,�Ƿ�ɿأ�2,����ǰ��¼��3����������¼��4����Ƶ, 5����������¼��6������ǰ��¼��7~12,ǰ������(���ɿأ�ǹ����=0,�ɿأ������=1,ץ��ǹ��=2,����=3,ץ�ĸ���=4)*/;
    unsigned int    longitude/*����*/;
    unsigned int    latitude/*γ��*/;
    unsigned int    reserve/*����״̬*/;
}NVIEW_SDK_CHANNEL_INFO, *PNVIEW_SDK_CHANNEL_INFO;

typedef struct
{
    char	flag;
    char	status;
    int		begin_time/*��ʼʱ��*/;
    int		end_time/*����ʱ��*/;
    char    qos;
    int		size;
    char	area[256]/*�洢����*/;
    char	name[256]/*�ļ���*/;
    char    deviceId[20];
    int     channelId;
}NVIEW_SDK_RECORD_INFO, *PNVIEW_SDK_RECORD_INFO;

typedef struct
{
    char	dev_channel[36]/*ͨ����ʽ��200000-200000000100011835-0001-0001*/;
    char	rule_guid[16]/*Rule's GUID*/;
    char	major_type/*�澯������*/;
    char	minor_type/*�澯������*/;
    char	alarm_level/*�澯���ؼ���(5��10��15��20��25��ֵԽ��Խ����)*/;
    char	alarm_confidence/*�澯���Ŷ�*/;
    int		alarm_session_id/*�澯�Ự��ţ�0,������1-0xFFFFFFFF*/;
    short	alarm_sequence_id/*�澯��ţ�0,����; n��N�θ澯��0xFFFF����*/;
    char	alarm_guid[16]/*�澯GUID��UAͨ����GUID���������ѯ����澯��Ϣ*/;
    char	content_type[16]/*�澯����ı�����*/;
    char	content[256]/*�澯����ı�����*/;
    int		alarm_flag/*�澯��־��1:���ڸ澯ͼƬ*/;
    int		alarm_time/*�澯ʱ��*/;
    char	storage_area_id[128]/*�澯ͼƬ�洢��AreaId*/;
}NVIEW_SDK_NOTIFY_ALARM_INFO;

typedef struct
{
    char    url[256];	//����URL
    void*   context;	//��������ʱ��������
}NVIEW_SDK_NOTIFY_DOWNLOAD_URL;

typedef struct
{
    char	dev_id[18]/*�豸ID��200000000100011835*/;
    bool	is_online/*����״̬*/;
}NVIEW_SDK_NOTIFY_FD_ONLINE;

//��Ϣ�ص�
typedef void(__cdecl *MESSAGE_CALLBACK)(void* identity, int msg_type, int error_code, void* msg_context);

//֪ͨ�ص�
typedef void(__cdecl *NOTIFY_CALLBACK)(void* identity, int notify_type, void* notify_info);

//¼�����صĻص���identity: SDK��ʼ��ʱ��downlod�����ؾ����percent�����ؽ��ȣ�context���û��þ�
typedef void(__cdecl *DOWNLOAD_PROC)(void* downlod, int percent, void *context);

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

    //��ʼ��
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Init(MESSAGE_CALLBACK message_cb, NOTIFY_CALLBACK notify_cb, void* identity);

    //����ʼ��
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Uninit();

    //=============================��¼=====================================
    NVIEW_SDK_API(HANDLE)   NVIEW_SDK_Login(const char* uas_ip, unsigned short uas_port, const char* email, const char* password);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_IsLogined(HANDLE nview_sdk);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Logout(HANDLE nview_sdk);

    //��Ƶ����ص�
    NVIEW_SDK_API(void)     NVIEW_SDK_SetMediaDisplayCallback(HANDLE nview_sdk, MEDIA_VIDEO_PROC callback_ptr);
    NVIEW_SDK_API(void)     NVIEW_SDK_SetMediaAudioCallback(HANDLE nview_sdk, MEDIA_AUDIO_PROC callback_ptr);
    NVIEW_SDK_API(void)     NVIEW_SDK_SetMediaNotifyCallback(HANDLE nview_sdk, MEDIA_NOTIFY_PROC callback_ptr);
    NVIEW_SDK_API(void)     NVIEW_SDK_SetMediaDataCallback(HANDLE nview_sdk, MEDIA_FRAME_PROC callback_ptr);

    NVIEW_SDK_API(int)      NVIEW_SDK_GetChannelCount(HANDLE nview_sdk);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_GetChannleList(HANDLE nview_sdk, PNVIEW_SDK_CHANNEL_INFO pChannelListBuf);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_GetChannleInfo(HANDLE nview_sdk, const char* dev_channel, NVIEW_SDK_CHANNEL_INFO& channel_info);
    NVIEW_SDK_API(int)      NVIEW_SDK_GetChannleStatus(HANDLE nview_sdk, const char* dev_channel);

    //=============================��Ƶ=====================================
    //������Ƶ
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StartMonitor(HANDLE nview_sdk, const char* dev_channel, int net_type, HWND monitor_wnd, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StopMonitor(HANDLE nview_sdk, const char* dev_channel, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_ControlCamera(HANDLE nview_sdk, const char* dev_channel, unsigned char action, unsigned char param, void* context);

    //��ȡ����ͳ����Ϣ
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_GetStatInfo(HANDLE nview_sdk, const char* dev_channel, TCH_STAT_INFO& tStatInfo);

    //����ץ��
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_SnapShot(HANDLE nview_sdk, const char* dev_channel, const char* path_file);

    //����¼��
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StartLocalRecord(HANDLE nview_sdk, const char* dev_channel, const char* path_file, long record_time, bool audio_on);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StopLocalRecord(HANDLE nview_sdk, const char* dev_channel);

    //��������Ƶ
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_EnableStream(HANDLE nview_sdk, const char* dev_channel, bool enable);

    //=============================¼��=====================================
    //��ѯ¼��
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_QueryRecord(HANDLE nview_sdk, const char* dev_channel, bool center, long start_time, long end_time, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_GetRecordList(HANDLE nview_sdk, PNVIEW_SDK_RECORD_INFO pRecordListBuf);

    //�����ط�
    NVIEW_SDK_API(HANDLE)   NVIEW_SDK_StartPlayback(HANDLE nview_sdk, const PNVIEW_SDK_RECORD_INFO pRecordInfo, int net_type, HWND playback_wnd, bool bCenter, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StopPlayback(HANDLE nview_sdk, HANDLE playback, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_ControlPlayback(HANDLE nview_sdk, HANDLE playback, unsigned char action, unsigned char param, void* context);

    //��ȡ����ͳ����Ϣ
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_GetStatInfo(HANDLE nview_sdk, HANDLE playback, TCH_STAT_INFO& tStatInfo);

    //����ץ��
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_SnapShot(HANDLE nview_sdk, HANDLE playback, const char* path_file);

    //����¼��
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_StartLocalRecord(HANDLE nview_sdk, HANDLE playback, const char* path_file, long record_time, bool audio_on);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_StopLocalRecord(HANDLE nview_sdk, HANDLE playback);

    //��������Ƶ
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_EnableStream(HANDLE nview_sdk, HANDLE playback, bool enable);

    NVIEW_SDK_API(BOOL)     NVIEW_SDK_QueryDownloadURL(HANDLE nview_sdk, const PNVIEW_SDK_RECORD_INFO pRecordInfo, void* context);
    NVIEW_SDK_API(HANDLE)   NVIEW_SDK_StartDownload(const char* record_url, const char* path_file, DOWNLOAD_PROC download_callback, void* context);
    NVIEW_SDK_API(void)     NVIEW_SDK_StopDownload(HANDLE download);

#ifdef __cplusplus
}
#endif /* __cplusplus */