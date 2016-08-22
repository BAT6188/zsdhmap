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
#define MSG_TYPE_START_AUDIO				11 //启动语音对讲消息
#define MSG_TYPE_STOP_AUDIO					MSG_TYPE_STOP_MONITOR //停止语音对讲消息

#define NOTIFY_TYPE_ALARM					100
#define NOTIFY_TYPE_FD_ONLINE				101
#define NOTIFY_TYPE_DOWNLOAD_URL			102

//net type
#define NET_TYPE_UDP				1
#define NET_TYPE_TCP				2

enum EM_PLAYBACK_CTRL_ACTION
{
    EM_PLAYBACK_CTRL_ACTION_FastForward = 0x01,//快进
    EM_PLAYBACK_CTRL_ACTION_FastBackward = 0x02,//快退
    EM_PLAYBACK_CTRL_ACTION_Pause = 0x03,//暂停 
    EM_PLAYBACK_CTRL_ACTION_Stop = 0x04,//停止 
    EM_PLAYBACK_CTRL_ACTION_Continue = 0x05,//继续回放 
    EM_PLAYBACK_CTRL_ACTION_DragPlay = 0x06,//回放拖动
    EM_PLAYBACK_CTRL_ACTION_SlowForward = 0x07,//慢进 
    EM_PLAYBACK_CTRL_ACTION_SlowBackward = 0x08,//慢退
    EM_PLAYBACK_CTRL_ACTION_FramePlay = 0x09,//单帧放
    EM_PLAYBACK_CTRL_ACTION_Forward = 0x0a,//以当前倍率向前放
    EM_PLAYBACK_CTRL_ACTION_Backward = 0x0b,//以当前倍率向后放
};

enum EM_CAMERA_CONTROL_ACTION
{
    EM_CAMERA_CONTROL_ACTION_UP = 0x00000001 /*向上转*/,
    EM_CAMERA_CONTROL_ACTION_LEFT = 0x00000002 /*向左转*/,
    EM_CAMERA_CONTROL_ACTION_ROTATE = 0x00000003 /*旋转*/,
    EM_CAMERA_CONTROL_ACTION_RIGHT = 0x00000004 /*向右转*/,
    EM_CAMERA_CONTROL_ACTION_DOWN = 0x00000005 /*向下转*/,
    EM_CAMERA_CONTROL_ACTION_FAR = 0x00000006 /*镜头拉远*/,
    EM_CAMERA_CONTROL_ACTION_FOCUSNEAR = 0x00000007 /*聚焦*/,
    EM_CAMERA_CONTROL_ACTION_AUTO = 0x00000008 /*自动*/,
    EM_CAMERA_CONTROL_ACTION_FOCUSFAR = 0x00000009 /*散焦*/,
    EM_CAMERA_CONTROL_ACTION_NEAR = 0x0000000a /*镜头拉近*/,
    EM_CAMERA_CONTROL_ACTION_DIAPHRAGM_LARGE = 0x0000000b /*光圈增大（变亮）*/,
    EM_CAMERA_CONTROL_ACTION_DIAPHRAGM_SMALL = 0x0000000c /*光圈减小（变暗）*/,
    EM_CAMERA_CONTROL_ACTION_STOP = 0x0000000d /*停止动作*/,
    EM_CAMERA_CONTROL_ACTION_SPEAKER_ON = 0x0000000e /*打开喇叭*/,
    EM_CAMERA_CONTROL_ACTION_LIGHT_ON = 0x0000000f /*打开灯光*/,
    EM_CAMERA_CONTROL_ACTION_HOTDOG = 0x00000010 /*热狗扫描*/,
    EM_CAMERA_CONTROL_ACTION_SPEAKER_OFF = 0x00000011 /*关闭喇叭*/,
    EM_CAMERA_CONTROL_ACTION_LIGHT_OFF = 0x00000012 /*关闭灯光*/,
    EM_CAMERA_CONTROL_ACTION_PRESET_GOTO = 0x00000013 /*切换到预置点*/,
    EM_CAMERA_CONTROL_ACTION_PRESET_SET = 0x00000014 /*设置预置点*/,
    EM_CAMERA_CONTROL_ACTION_PRESET_DEL = 0x00000015 /*删除预置点*/,
    EM_CAMERA_CONTROL_ACTION_CAMERA_RESET = 0x00000016 /*摄像机复位*/,
    EM_CAMERA_CONTROL_ACTION_WIPER_ON = 0x00000017 /*打开雨刷*/,
    EM_CAMERA_CONTROL_ACTION_WIPER_OFF = 0x00000018 /*关闭雨刷*/,
    EM_CAMERA_CONTROL_ACTION_AUTOCRUISE = 0x0000001d /*自动巡航*/,
    EM_CAMERA_CONTROL_ACTION_PRESET_CLEAR = 0x0000001e /*清除所有预置点*/,
    EM_CAMERA_CONTROL_ACTION_STARTTRACKING = 0x0000001f /*启动跟踪*/,
    EM_CAMERA_CONTROL_ACTION_STOPTRACKING = 0x00000020 /*停止跟踪*/,
    EM_CAMERA_CONTROL_ACTION_LEFTUP = 0x00000021 /*左上转*/,
    EM_CAMERA_CONTROL_ACTION_RIGHTUP = 0x00000022 /*右上转*/,
    EM_CAMERA_CONTROL_ACTION_LEFTDOWN = 0x00000023 /*左下转*/,
    EM_CAMERA_CONTROL_ACTION_RIGHTDOWN = 0x00000024 /*右下转*/,
    EM_CAMERA_CONTROL_ACTION_CAMERAACTIVE = 0x00000028 /*摄像机激活*/,
    EM_CAMERA_CONTROL_ACTION_SETPANSPEED = 0x00000029 /*设置左右转动速度*/,
    EM_CAMERA_CONTROL_ACTION_SETTILTSPEED = 0x00000030 /*设置上下转动速度*/,
    EM_CAMERA_CONTROL_ACTION_SETZOOMSPEED = 0x00000031 /*设置光圈速度*/,
    EM_CAMERA_CONTROL_ACTION_SETFOCUSSPEED = 0x00000032 /*设置聚焦/散焦速度*/,
    EM_CAMERA_CONTROL_ACTION_SPEEDSETTINGGET = 0x00000033 /*获取摄像机速度设置*/,
    EM_CAMERA_CONTROL_ACTION_MATRIXSWITCH = 0x00000040 /*矩阵切换*/,
    EM_CAMERA_CONTROL_ACTION_BRIGHTNESS = 0x00000050 /*亮度*/,
    EM_CAMERA_CONTROL_ACTION_CONTRAST = 0x00000051 /*对比度*/,
    EM_CAMERA_CONTROL_ACTION_SATURATION = 0x00000052 /*饱和度*/,
    EM_CAMERA_CONTROL_ACTION_HUE = 0x00000053 /*色度*/,
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
    NVIEW_SDK_ERROR_COMMON_USER_CANCELED = 0x80010016 /*操作已被用户取消，例如发送STOP*/,
    NVIEW_SDK_ERROR_COMMON_NOTSUPPORT = 0x80010017 /*功能不支持*/,
    NVIEW_SDK_ERROR_COMMON_RECORD_TIME = 0x80010018,
    NVIEW_SDK_ERROR_COMMON_REACH_MAX = 0x80010019,
    NVIEW_SDK_ERROR_COMMON_CSS_NOSPACE = 0x8001001a /*中心存储器上的总空间为0*/,
    NVIEW_SDK_ERROR_COMMON_MSS_INVALID_PARAM = 0x8001001b,
    NVIEW_SDK_ERROR_COMMON_REACH_CUSTOMERMAX = 0x8001001c,
    NVIEW_SDK_ERROR_COMMON_TIME = 0x8001001d /*时间错误，在抓拍、录像等需要指定时间的指令中，时间无效*/,
    NVIEW_SDK_ERROR_COMMON_LOCKED = 0x8001001e /*设备被锁住，在云台控制命令中，云台被其他用户锁住*/,
    NVIEW_SDK_ERROR_COMMON_REMOTE_DOMAIN_OFFLINE = 0x8001001f /*跨域服务器未在线*/,
    NVIEW_SDK_ERROR_COMMON_VERSION_TOOLD = 0x80010020 /*版本太旧*/,
    NVIEW_SDK_ERROR_COMMON_BUSY = 0x80010021 /*太忙*/,
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
    NVIEW_SDK_ERROR_AAA_UNKNOWN = 0x80040001 /*未知原因错误*/,
    NVIEW_SDK_ERROR_AAA_INVALIDPACKET = 0x80040002 /*Radius包不合法*/,
    NVIEW_SDK_ERROR_AAA_MISSATTRIBUTE = 0x80040003 /*Radius包缺少必要的属性*/,
    NVIEW_SDK_ERROR_AAA_INTERNAL = 0x80040004 /*AAA服务器发生内部错误*/,
    NVIEW_SDK_ERROR_AAA_DBMS = 0x80040005 /*AAA服务器操作数据库失败*/,
    NVIEW_SDK_ERROR_AAA_STOPPED = 0x80040006 /*AAA服务器停止服务*/,
    NVIEW_SDK_ERROR_AAA_PASSWORD = 0x80040007 /*密码错误*/,
    NVIEW_SDK_ERROR_AAA_BADUSERID = 0x80040008 /*用户ID不存在*/,
    NVIEW_SDK_ERROR_AAA_BADDEVID = 0x80040009 /*设备ID不存在*/,
    NVIEW_SDK_ERROR_AAA_BADCHANNEL = 0x8004000a /*设备通道号不存在*/,
    NVIEW_SDK_ERROR_AAA_FORBIDDEN = 0x8004000b /*没有权限执行所请求的操作*/,
    NVIEW_SDK_ERROR_AAA_NOMONEY = 0x8004000c /*费用不足*/,
    NVIEW_SDK_ERROR_AAA_NODISK = 0x8004000d /*磁盘配额不足*/,
    NVIEW_SDK_ERROR_AAA_CUSTOMERSTATUS = 0x8004000e /*客户状态不正常*/,
    NVIEW_SDK_ERROR_AAA_USERSTATUS = 0x8004000f /*用户状态不正常*/,
    NVIEW_SDK_ERROR_AAA_USERIPDENIED = 0x80040010 /*用户IP地址拒绝*/,
    NVIEW_SDK_ERROR_AAA_SMSCODEFAIL = 0x80040011 /*短信登录次数超出*/,
    NVIEW_SDK_ERROR_AAA_SMSBAD = 0x80040012 /*短信验证码错误*/,
    NVIEW_SDK_ERROR_AAA_SMSSENDFAILED = 0x80040013 /*短信验证码发送失败*/,
    NVIEW_SDK_ERROR_MSS_BASE = 0x80050000,
    NVIEW_SDK_ERROR_MSS_FAIL = 0x80050001 /*:-1         失败*/,
    NVIEW_SDK_ERROR_MSS_INVALID_ID = 0x80050002 /*:-2         非法ID*/,
    NVIEW_SDK_ERROR_MSS_INVALID_HANDLE = 0x80050003 /*:-3         非法句柄*/,
    NVIEW_SDK_ERROR_MSS_INVALID_PARAM = 0x80050004 /*:-4         非法参数*/,
    NVIEW_SDK_ERROR_MSS_NULLPTR = 0x80050005 /*:-5         指针为NULL*/,
    NVIEW_SDK_ERROR_MSS_REPEAT = 0x80050006 /*:-6         重复操作*/,
    NVIEW_SDK_ERROR_MSS_TIMEOUT = 0x80050007 /*:-7         超时*/,
    NVIEW_SDK_ERROR_MSS_OUTOF_MEMORY = 0x80050008 /*:-8         内存不足*/,
    NVIEW_SDK_ERROR_MSS_OUTOF_RANGE = 0x80050009 /*:-9         超出范围*/,
    NVIEW_SDK_ERROR_MSS_FULL = 0x8005000a /*:-10        缓冲区满*/,
    NVIEW_SDK_ERROR_MSS_UNEXPECTED = 0x8005000b /*:-11        未知异常*/,
    NVIEW_SDK_ERROR_MSS_PRIVILEGE_LIMIT = 0x8005000c /*:-12        权限限制*/,
    NVIEW_SDK_ERROR_MSS_BUSY = 0x80050100 /*MSS服务忙*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_FD = 0x80050101 /*MSS调度FD失败*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_RTMDS = 0x80050102 /*MSS调度RTMDS失败*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_SMS = 0x80050103 /*MSS调度SMS失败*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_FTS = 0x80050104 /*MSS调度FTS失败*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_CSS = 0x80050105 /*MSS调度CSS失败*/,
    NVIEW_SDK_ERROR_MSS_SCHEDULE_CANCELLED = 0x80050106 /*MSS调度被取消*/,
    NVIEW_SDK_ERROR_MSS_FD_OFFLINE = 0x80050200 /*FD未在线, 或在VPN内*/,
    NVIEW_SDK_ERROR_MSS_FD_CHANNEL_INVALID = 0x80050201 /*FD未在线*/,
    NVIEW_SDK_ERROR_MSS_FD_BUSY = 0x80050202 /*FD忙*/,
    NVIEW_SDK_ERROR_MSS_FD_NO_RIGHT = 0x80050203 /*FD访问未被授权*/,
    NVIEW_SDK_ERROR_MSS_FD_FAILTURE = 0x80050204 /*FD操作失败*/,
    NVIEW_SDK_ERROR_MSS_FD_RETURN_ERROR = 0x80050205 /*FD返回错误*/,
    NVIEW_SDK_ERROR_MSS_FD_PACKAGE_ERROR = 0x80050206 /*FD打包错误*/,
    NVIEW_SDK_ERROR_MSS_FDMS_OFFLINE = 0x80050300 /*FDMS服务未在线 0x300*/,
    NVIEW_SDK_ERROR_MSS_FDMS_TIMEOUT = 0x80050301 /*FDMS服务响应超时*/,
    NVIEW_SDK_ERROR_MSS_FDMS_FAILTURE = 0x80050302 /*FDMS服务响应失败*/,
    NVIEW_SDK_ERROR_MSS_RTMDS_OFFLINE = 0x80050400 /*RTMDS服务未在线 0x400*/,
    NVIEW_SDK_ERROR_MSS_RTMDS_TIMEOUT = 0x80050401 /*RTMDS服务响应超时*/,
    NVIEW_SDK_ERROR_MSS_RTMDS_FAILTURE = 0x80050402 /*RTMDS服务响应失败*/,
    NVIEW_SDK_ERROR_MSS_SMS_OFFLINE = 0x80050500 /*SMS服务未在线  0x500*/,
    NVIEW_SDK_ERROR_MSS_SMS_TIMEOUT = 0x80050501 /*SMS服务响应超时*/,
    NVIEW_SDK_ERROR_MSS_SMS_FAILTURE = 0x80050502 /*SMS服务响应失败*/,
    NVIEW_SDK_ERROR_MSS_SMS_NO_FILE = 0x80050503 /*SMS服务响应文件不存在*/,
    NVIEW_SDK_ERROR_MSS_FTS_OFFLINE = 0x80050600 /*FTS服务未在线 0x600*/,
    NVIEW_SDK_ERROR_MSS_FTS_TIMEOUT = 0x80050601 /*FTS服务响应超时*/,
    NVIEW_SDK_ERROR_MSS_FTS_FAILTURE = 0x80050602 /*FTS服务响应失败*/,
    NVIEW_SDK_ERROR_MSS_FTS_NO_FILE = 0x80050603 /*FTS服务响应文件不存在*/,
    NVIEW_SDK_ERROR_MSS_CSS_OFFLINE = 0x80050700 /*CSS服务未在线 0x700*/,
    NVIEW_SDK_ERROR_MSS_CSS_TIMEOUT = 0x80050701 /*CSS服务响应超时*/,
    NVIEW_SDK_ERROR_MSS_CSS_FAILTURE = 0x80050702 /*CSS服务响应失败*/,
    NVIEW_SDK_ERROR_MSS_CSS_NO_FILE = 0x80050703 /*CSS服务响应文件不存在*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORDING = 0x80050704 /*CSS服务响应正在录像*/,
    NVIEW_SDK_ERROR_MSS_CSS_BUSY = 0x80050705 /*CSS服务响应忙*/,
    NVIEW_SDK_ERROR_MSS_CSS_DISK_FULL = 0x80050706 /*CSS服务响应磁盘已满*/,
    NVIEW_SDK_ERROR_MSS_CSS_DISK_QUOTA = 0x80050707 /*CSS服务响应磁盘限额*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_STOP = 0x80050708 /*CSS服务报告录像停止*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_EXPIRE = 0x80050709 /*CSS服务报告录像时间到达*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_QUOTA = 0x8005070a /*CSS服务报告录像磁盘限额满*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_NETWORKIO = 0x8005070b /*CSS服务报告录像网络IO错误*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_FILEIO = 0x8005070c /*CSS服务报告录像文件IO错误*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_STOPBYCMS = 0x8005070d /*CSS服务报告录像被系统管理员停止*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_NOPACKET = 0x8005070e /*CSS服务报告录像在一定的秒内接收到的数据包太少*/,
    NVIEW_SDK_ERROR_MSS_CSS_RECORD_NOFRAME = 0x8005070f /*CSS服务报告录像在一定的秒内接收到的数据包所组成的帧数为0*/,
    NVIEW_SDK_ERROR_MSS_SSU_OFFLINE = 0x80050d00,
    NVIEW_SDK_ERROR_MSS_SSU_TIMEOUT = 0x80050d01,
    NVIEW_SDK_ERROR_MSS_SSU_FAILTURE = 0x80050d02,
    NVIEW_SDK_ERROR_MSS_SID_MISMATCH = 0x80059000 /*返回来的sessionid不匹配*/,
    NVIEW_SDK_ERROR_DBMS_BASE = 0x80060000,
    NVIEW_SDK_ERROR_DBMS_QUERY_FD_DOMAIN = 0x80060001,
    NVIEW_SDK_ERROR_DBMS_CUSTOM_NOTEXISTS = 0x80060002 /*DBMS查询客户不存在*/,
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
    NVIEW_SDK_ERROR_CMS_NAMEID_INDEX_OVERFLOW = 0x80080001 /*下标层次越界*/,
    NVIEW_SDK_ERROR_CMS_ID_NOTEXIST = 0x80080002 /*对象不存在*/,
    NVIEW_SDK_ERROR_CMS_TRAP_NOTSETABLE = 0x80080003 /*对象不可设置*/,
    NVIEW_SDK_ERROR_CMS_CLASS_NOTEXIST = 0x80080004 /*类不存在*/,
    NVIEW_SDK_ERROR_CMS_METHOD_NOTEXIST = 0x80080005 /*方法不存在*/,
    NVIEW_SDK_ERROR_CMS_MEMBER_NOTEXIST = 0x80080006 /*成员不存在*/,
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
    char            channel_name[128]/*通道名称*/;
    char            channel_code[36]/*通道格式：200000-200000000100011835-0001-0001*/;
    char            db33_code[19]/*DB33编号*/;
    char            gab_code[21]/*国标编号*/;
    unsigned char   isonline/*设备在线状态*/;
    unsigned short  factory_code/*厂商代码，查看视频的时候就不用再查详细信息了*/;
    unsigned int    channel_status/*BIT 1,是否可控，2,正在前端录像，3，正在中心录像，4，视频, 5，启用中心录像，6，启用前端录像，7~12,前端类型(不可控（枪机）=0,可控（球机）=1,抓拍枪机=2,高清=3,抓拍高清=4)*/;
    unsigned int    longitude/*经度*/;
    unsigned int    latitude/*纬度*/;
    unsigned int    reserve/*保留状态*/;
}NVIEW_SDK_CHANNEL_INFO, *PNVIEW_SDK_CHANNEL_INFO;

typedef struct
{
    char	flag;
    char	status;
    int		begin_time/*起始时间*/;
    int		end_time/*结束时间*/;
    char    qos;
    int		size;
    char	area[256]/*存储区域*/;
    char	name[256]/*文件名*/;
    char    deviceId[20];
    int     channelId;
}NVIEW_SDK_RECORD_INFO, *PNVIEW_SDK_RECORD_INFO;

typedef struct
{
    char	dev_channel[36]/*通道格式：200000-200000000100011835-0001-0001*/;
    char	rule_guid[16]/*Rule's GUID*/;
    char	major_type/*告警主类型*/;
    char	minor_type/*告警子类型*/;
    char	alarm_level/*告警严重级别(5、10、15、20、25数值越大越严重)*/;
    char	alarm_confidence/*告警置信度*/;
    int		alarm_session_id/*告警会话编号，0,保留，1-0xFFFFFFFF*/;
    short	alarm_sequence_id/*告警序号，0,保留; n第N次告警，0xFFFF销警*/;
    char	alarm_guid[16]/*告警GUID，UA通过此GUID向服务器查询具体告警信息*/;
    char	content_type[16]/*告警相关文本类型*/;
    char	content[256]/*告警相关文本内容*/;
    int		alarm_flag/*告警标志：1:存在告警图片*/;
    int		alarm_time/*告警时间*/;
    char	storage_area_id[128]/*告警图片存储的AreaId*/;
}NVIEW_SDK_NOTIFY_ALARM_INFO;

typedef struct
{
    char    url[256];	//下载URL
    void*   context;	//请求下载时的上下文
}NVIEW_SDK_NOTIFY_DOWNLOAD_URL;

typedef struct
{
    char	dev_id[18]/*设备ID：200000000100011835*/;
    bool	is_online/*在线状态*/;
}NVIEW_SDK_NOTIFY_FD_ONLINE;

//消息回调
typedef void(__cdecl *MESSAGE_CALLBACK)(void* identity, int msg_type, int error_code, void* msg_context);

//通知回调
typedef void(__cdecl *NOTIFY_CALLBACK)(void* identity, int notify_type, void* notify_info);

//录像下载的回调，identity: SDK初始化时，downlod：下载句柄；percent：下载进度；context：用户用据
typedef void(__cdecl *DOWNLOAD_PROC)(void* downlod, int percent, void *context);

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

    //初始化
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Init(MESSAGE_CALLBACK message_cb, NOTIFY_CALLBACK notify_cb, void* identity);

    //反初始化
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Uninit();

    //=============================登录=====================================
    NVIEW_SDK_API(HANDLE)   NVIEW_SDK_Login(const char* uas_ip, unsigned short uas_port, const char* email, const char* password);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_IsLogined(HANDLE nview_sdk);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Logout(HANDLE nview_sdk);

    //视频解码回调
    NVIEW_SDK_API(void)     NVIEW_SDK_SetMediaDisplayCallback(HANDLE nview_sdk, MEDIA_VIDEO_PROC callback_ptr);
    NVIEW_SDK_API(void)     NVIEW_SDK_SetMediaAudioCallback(HANDLE nview_sdk, MEDIA_AUDIO_PROC callback_ptr);
    NVIEW_SDK_API(void)     NVIEW_SDK_SetMediaNotifyCallback(HANDLE nview_sdk, MEDIA_NOTIFY_PROC callback_ptr);
    NVIEW_SDK_API(void)     NVIEW_SDK_SetMediaDataCallback(HANDLE nview_sdk, MEDIA_FRAME_PROC callback_ptr);

    NVIEW_SDK_API(int)      NVIEW_SDK_GetChannelCount(HANDLE nview_sdk);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_GetChannleList(HANDLE nview_sdk, PNVIEW_SDK_CHANNEL_INFO pChannelListBuf);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_GetChannleInfo(HANDLE nview_sdk, const char* dev_channel, NVIEW_SDK_CHANNEL_INFO& channel_info);
    NVIEW_SDK_API(int)      NVIEW_SDK_GetChannleStatus(HANDLE nview_sdk, const char* dev_channel);

    //=============================视频=====================================
    //开启视频
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StartMonitor(HANDLE nview_sdk, const char* dev_channel, int net_type, HWND monitor_wnd, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StopMonitor(HANDLE nview_sdk, const char* dev_channel, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_ControlCamera(HANDLE nview_sdk, const char* dev_channel, unsigned char action, unsigned char param, void* context);

    //获取码流统计信息
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_GetStatInfo(HANDLE nview_sdk, const char* dev_channel, TCH_STAT_INFO& tStatInfo);

    //本地抓拍
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_SnapShot(HANDLE nview_sdk, const char* dev_channel, const char* path_file);

    //本地录象
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StartLocalRecord(HANDLE nview_sdk, const char* dev_channel, const char* path_file, long record_time, bool audio_on);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StopLocalRecord(HANDLE nview_sdk, const char* dev_channel);

    //开启音视频
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_EnableStream(HANDLE nview_sdk, const char* dev_channel, bool enable);

    //=============================录像=====================================
    //查询录像
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_QueryRecord(HANDLE nview_sdk, const char* dev_channel, bool center, long start_time, long end_time, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_GetRecordList(HANDLE nview_sdk, PNVIEW_SDK_RECORD_INFO pRecordListBuf);

    //启动回放
    NVIEW_SDK_API(HANDLE)   NVIEW_SDK_StartPlayback(HANDLE nview_sdk, const PNVIEW_SDK_RECORD_INFO pRecordInfo, int net_type, HWND playback_wnd, bool bCenter, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_StopPlayback(HANDLE nview_sdk, HANDLE playback, void* context);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_ControlPlayback(HANDLE nview_sdk, HANDLE playback, unsigned char action, unsigned char param, void* context);

    //获取码流统计信息
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_GetStatInfo(HANDLE nview_sdk, HANDLE playback, TCH_STAT_INFO& tStatInfo);

    //本地抓拍
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_SnapShot(HANDLE nview_sdk, HANDLE playback, const char* path_file);

    //本地录象
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_StartLocalRecord(HANDLE nview_sdk, HANDLE playback, const char* path_file, long record_time, bool audio_on);
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_StopLocalRecord(HANDLE nview_sdk, HANDLE playback);

    //开启音视频
    NVIEW_SDK_API(BOOL)     NVIEW_SDK_Playback_EnableStream(HANDLE nview_sdk, HANDLE playback, bool enable);

    NVIEW_SDK_API(BOOL)     NVIEW_SDK_QueryDownloadURL(HANDLE nview_sdk, const PNVIEW_SDK_RECORD_INFO pRecordInfo, void* context);
    NVIEW_SDK_API(HANDLE)   NVIEW_SDK_StartDownload(const char* record_url, const char* path_file, DOWNLOAD_PROC download_callback, void* context);
    NVIEW_SDK_API(void)     NVIEW_SDK_StopDownload(HANDLE download);

#ifdef __cplusplus
}
#endif /* __cplusplus */