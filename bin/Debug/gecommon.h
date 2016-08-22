#ifndef __GE_COMMON_INCLUDE_H__
#define __GE_COMMON_INCLUDE_H__

#ifdef __cplusplus
extern "C" {
#endif	// __cplusplus

//codec type
#define MEDIA_CODECTYPE_AUDIO_ENCODE	(0x00000001)			// 音频编码
#define MEDIA_CODECTYPE_AUDIO_DECODE	(0x00000002)			// 音频解码
#define MEDIA_CODECTYPE_VIDEO_ENCODE	(0x00000004)			// 视频编码
#define MEDIA_CODECTYPE_VIDEO_DECODE	(0x00000008)			// 视频解码

//net type
#define MEDIA_NETTYPE_UDP				(0x00000001)			// UDP客户端
#define MEDIA_NETTYPE_TCP				(0x00000002)			// TCP客户端
#define MEDIA_NETTYPE_BLOCK				(0x00000004)			// 为TCP回放、下载无丢帧的连接策略
#define MEDIA_NETTYPE_NORMAL			(0x00000000)//普通网络
#define MEDIA_NETTYPE_3G				(0x00000100)//表示3G等无线网络，需要启用丢包统计回传等策略 在MEDIA_NETTYPE_UDP 或上此宏即可
#define MEDIA_NETTYPE_ZB				(0x00010000)//表示浙江贝的RTP
#define MEDIA_NETTYPE_RTP_DB33			(0x00020000)//表示DB33的RTP
#define MEDIA_NETTYPE_NODECODE			(0x00100000)//不解码标示,用于仅仅录像
#define MEDIA_NETTYPE_PLAYBACK			(0x01000000)//录像回放标识，以区分实时视频请求

//factory type
#define MEDIA_FACTORYTYPE_ME			0x00010000		//星望
#define MEDIA_FACTORYTYPE_HK			0x00020000		//海康
#define MEDIA_FACTORYTYPE_SH			0x00040000		//上海测试
#define MEDIA_FACTORYTYPE_DH			0x00080000		//大华
#define MEDIA_FACTORYTYPE_ZB			0x00100000		//杭卡
#define MEDIA_FACTORYTYPE_DL			0x00200000		//大立
#define MEDIA_FACTORYTYPE_ST			0x00400000		//三立
#define MEDIA_FACTORYTYPE_FH			0x00800000		//烽火
#define MEDIA_FACTORYTYPE_HX			0x01000000		//虹信
#define MEDIA_FACTORYTYPE_ZB2			0x02000000		//杭卡
#define MEDIA_FACTORYTYPE_H3C			0x04000000		//华三
#define MEDIA_FACTORYTYPE_XC			0x08000000		//信产
#define MEDIA_FACTORYTYPE_XW            0x10000000      //星望（老版本)
#define MEDIA_FACTORYTYPE_ZV            0x12000000      //南京北路
#define MEDIA_FACTORYTYPE_JL            0x20000000      //金陵
#define MEDIA_FACTORYTYPE_SR            0x40000000      //数尔
#define MEDIA_FACTORYTYPE_DB33V2		0x80000000		//DB33V2所有厂家

//encode type
#define MEDIA_ENCODETYPE_MPEG4			0x00000001
#define MEDIA_ENCODETYPE_H264			0x00000002
#define MEDIA_ENCODETYPE_G711A			0x00000100
#define MEDIA_ENCODETYPE_G711U			0x00000200
#define MEDIA_ENCODETYPE_G7221			0x00000300
#define MEDIA_ENCODETYPE_G7231			0x00000400
#define MEDIA_ENCODETYPE_G726			0x00000500

//color space type
#define MEDIA_CSTYPE_YV12				0
#define MEDIA_CSTYPE_YUY2				1
#define MEDIA_CSTYPE_UYVY				2
#define MEDIA_CSTYPE_I420				3
#define MEDIA_CSTYPE_RGB24				4
#define MEDIA_CSTYPE_YVYU				5
#define MEDIA_CSTYPE_RGB32				6
#define MEDIA_CSTYPE_RGB555				7
#define MEDIA_CSTYPE_RGB565				8

//notify event
#define LOCAL_RECORD_STOP				101	//录像自动停止通知
#define DECODE_FIRST_FRAME				102	//收到并解码第一帧视频
#define LOCAL_STREAM_STOP				105	//收到对方的结束通道信令包
#define LOCAL_STREAM_NULL				106	//本地帧回放模式缓冲区空的通知
#define LOCAL_FLAG_FRAME				107	//本地帧回放模式解到标识帧通知

	typedef enum
	{
		HW_DISPLAY_FORMAT_INVALID		= 0x00000000,
		HW_DISPLAY_FORMAT_CVBS			= 0x00000001,
		HW_DISPLAY_FORMAT_DVI			= 0x00000002,
		HW_DISPLAY_FORMAT_VGA			= 0x00000004,	
		HW_DISPLAY_FORMAT_HDMI			= 0x00000008,	
		HW_DISPLAY_FORMAT_YPbPr			= 0x00000010
	}HW_DISPLAY_FORMAT;

	/*resolution*/
	typedef enum
	{
		HW_DISPLAY_RESOLUTION_INVALID			= 0x00000000,
		HW_DISPLAY_RESOLUTION_D1				= 0x00000001,
		HW_DISPLAY_RESOLUTION_XGA_60HZ			= 0x00000002,
		HW_DISPLAY_RESOLUTION_SXGA_60HZ			= 0x00000004,
		HW_DISPLAY_RESOLUTION_SXGA_960_60HZ		= 0x00000008,
		HW_DISPLAY_RESOLUTION_720P_50HZ			= 0x00000010,
		HW_DISPLAY_RESOLUTION_720P_60HZ			= 0x00000020,
		HW_DISPLAY_RESOLUTION_1080I_50HZ		= 0x00000040,
		HW_DISPLAY_RESOLUTION_1080I_60HZ		= 0x00000080,
		HW_DISPLAY_RESOLUTION_1080P_24HZ		= 0x00000100,
		HW_DISPLAY_RESOLUTION_1080P_25HZ		= 0x00000200,
		HW_DISPLAY_RESOLUTION_1080P_30HZ		= 0x00000400,
		HW_DISPLAY_RESOLUTION_1080P_60HZ		= 0x00000800,
		HW_DISPLAY_RESOLUTION_UXGA_60HZ			= 0x00001000
	}HW_DISPLAY_RESOLUTION;


/* 统计信息结构 */
typedef struct tagTCH_STAT_INFO
{
	UINT		unSize;						//本结构体长度, sizeof
	UINT		unTimeStamp;				//统计时刻，GetTickCount
	UINT		unTimeDiff;					//统计时长,ms
	UINT		unBitRate;					//比特率B/s
	UINT		unLostRate;					//丢包率，丢包万分比
	UINT		unFrameRate1;				//读取视频帧帧率 fps
	UINT		unFrameRate2;				//解码器回调帧率 fps

	USHORT		usWidth;					// 视频宽度
	USHORT		usHeight;					// 视频高度

	int			nStartTime;					//通道开始时间，time-t
	int			nConnectTime;				//通道连接建立时间，time-t

	unsigned long long	u64RecvPacket1;		//自通道建立后网络上收到的包数目
	unsigned long long	u64RecvByte1;		//自通道建立后网络上收到的字节数
	unsigned long long	u64RecvPacket2;		//自通道建立后协议层收到的有效包数目
	unsigned long long	u64RecvByte2;		//自通道建立后协议层收到的有效字节数

	UINT		unAudioType;				// 音频编码类型v2
	UINT		unVideoType;				// 视频编码类型v2，如(MAKEFOURCC('S', 'V', 'M', '4')) 表示 星望mpeg4
	UCHAR		ucAudioType;				// 音频编码类型
	UCHAR		ucVideoType;				// 视频编码类型

	USHORT		usRealLocalPort;			//本地使用的真实端口
	USHORT		usLocalPort;				//本地绑定的端口
	USHORT		usPeerPort;					//对方端口
	char		szRealLocalAddr[64];		//本地真实地址
	char		szLocalAddr[64];			//本地绑定地址
	char		szPeerAddr[64];				//对方地址

	char		szScokType[8];				//SOCKET 类型 tcp,udp...

}TCH_STAT_INFO, *PTCH_STAT_INFO;

typedef struct tagConfigData
{
	char	szName[32];
	char	szValue[32];
}TCfgData, *LPTCfgData;

/* 版本信息结构 */
typedef struct
{
	unsigned	unMouduleLevel;		// 用来标示树级别
	char			szModuleName[32];	//模块名
	unsigned	unVer;				//0xyy.yy.yyyy版本号
	int				nTime;				// compile time_t
	char			szVerInfo[128];
	char			szDescribe[256];
}MEDIA_VERSION_INFO;

typedef struct tagtFrameParam
{
	bool			bAgain;				//保留变量
	bool			bIsIFrame;			//是否为I帧
	BYTE			byType;				//类型:音频或视频 AUDIO_TYPE 或 VIDEO_TYPE
	BYTE			byFrameRate;		//帧率 fps
	UINT			unCodecType;		//编码类型v2，如(MAKEFOURCC('S', 'V', 'M', '4')) 表示 星望mpeg4
	USHORT			usWidth;			//图像宽度
	USHORT			usHeight;			//图像高度
	USHORT			usFactoryCode;		//厂家代码V2，0x686b表示"hk"
	unsigned	unTimeStamp;		//32位时戳，单位为ms，从0开始相对时间
	unsigned	dwFrameSeq;			//帧序列号，每帧加1
	unsigned long long	un64Timestamp;	//64位时戳，单位为ms，从19700101 00:00:00开始相对时间，主要用于回放时进度条的定位
	unsigned	unOrgTimeStamp;		//从包头里取出的未修改的时戳
}TFrameParam;

/* 媒体参数结构 */
typedef struct tagTMediaParam
{
	bool			bAgain;				//保留变量
	BYTE			byType;				//类型:音频或视频 AUDIO_TYPE 或 VIDEO_TYPE
	BYTE			byEncodeType;		//编码类型，单字节的码流类型
	bool			bIsIFrame;			//是否为I帧
	BYTE			byFrameRate;		//帧率 fps
	BYTE			byImageSize;		//图像分辨率
	unsigned	unTimeStamp;		//32位时戳，单位为ms，从0开始相对时间
	unsigned	dwFrameSeq;			//帧序列号，每帧加1
}TMediaParam;

//事件通知回调，录像已停止，解码第一帧完成等事件回调，unEventId为消息ID，参见notify event；pParam：用户用据
typedef	int (__cdecl *MEDIA_NOTIFY_PROC)(unsigned unEventId, void* pParam);

//解码前的音视频编码数据回调，pBuf：帧数据；nLen：数据长度；pFrameParam：相关参数；pParam：用户用据
typedef	int (__cdecl *MEDIA_FRAME_PROC)(unsigned char* pBuf, int nLen, TFrameParam* pFrameParam, void* pParam);

//解码后的视频数据回调，pBuf：视频数据；nLen：数据长度；nWidth：宽度；nHeight：高度；
//nTime：时间戳，定义同time_t；nCSType：颜色空间类型 color space type；pParam：用户用据
typedef	int (__cdecl *MEDIA_VIDEO_PROC)(unsigned char* pBuf, int nLen, int nWidth, int nHeight, int nTime, int nCSType, void* pParam);

//解码后的音频数据回调，pBuf：音频数据；nLen：数据长度；lpAudioFormat：音频格式，换为WAVEFORMATEX*使用；
//unTimeStamp：时间戳，单位ms；pParam：用户用据
typedef	int (__cdecl *MEDIA_AUDIO_PROC)(unsigned char* pBuf, int nLen, void* lpAudioFormat, unsigned unTimeStamp, void* pParam);

//编码后的音频数据回调，pBuf：编码后音频数据；nLen：数据长度；unCodecType：音频格式；unTimeStamp：时间戳，单位ms；pParam：用户用据
typedef	int (__cdecl *MEDIA_AUDIO_ENCODE_PROC)(unsigned char* pBuf, int nLen, unsigned unCodecType, unsigned unTimeStamp, void* pParam);

#ifdef __cplusplus
}
#endif	// __cplusplus

#endif	//__GE_COMMON_INCLUDE_H__
