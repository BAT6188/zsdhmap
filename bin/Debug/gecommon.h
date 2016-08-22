#ifndef __GE_COMMON_INCLUDE_H__
#define __GE_COMMON_INCLUDE_H__

#ifdef __cplusplus
extern "C" {
#endif	// __cplusplus

//codec type
#define MEDIA_CODECTYPE_AUDIO_ENCODE	(0x00000001)			// ��Ƶ����
#define MEDIA_CODECTYPE_AUDIO_DECODE	(0x00000002)			// ��Ƶ����
#define MEDIA_CODECTYPE_VIDEO_ENCODE	(0x00000004)			// ��Ƶ����
#define MEDIA_CODECTYPE_VIDEO_DECODE	(0x00000008)			// ��Ƶ����

//net type
#define MEDIA_NETTYPE_UDP				(0x00000001)			// UDP�ͻ���
#define MEDIA_NETTYPE_TCP				(0x00000002)			// TCP�ͻ���
#define MEDIA_NETTYPE_BLOCK				(0x00000004)			// ΪTCP�طš������޶�֡�����Ӳ���
#define MEDIA_NETTYPE_NORMAL			(0x00000000)//��ͨ����
#define MEDIA_NETTYPE_3G				(0x00000100)//��ʾ3G���������磬��Ҫ���ö���ͳ�ƻش��Ȳ��� ��MEDIA_NETTYPE_UDP ���ϴ˺꼴��
#define MEDIA_NETTYPE_ZB				(0x00010000)//��ʾ�㽭����RTP
#define MEDIA_NETTYPE_RTP_DB33			(0x00020000)//��ʾDB33��RTP
#define MEDIA_NETTYPE_NODECODE			(0x00100000)//�������ʾ,���ڽ���¼��
#define MEDIA_NETTYPE_PLAYBACK			(0x01000000)//¼��طű�ʶ��������ʵʱ��Ƶ����

//factory type
#define MEDIA_FACTORYTYPE_ME			0x00010000		//����
#define MEDIA_FACTORYTYPE_HK			0x00020000		//����
#define MEDIA_FACTORYTYPE_SH			0x00040000		//�Ϻ�����
#define MEDIA_FACTORYTYPE_DH			0x00080000		//��
#define MEDIA_FACTORYTYPE_ZB			0x00100000		//����
#define MEDIA_FACTORYTYPE_DL			0x00200000		//����
#define MEDIA_FACTORYTYPE_ST			0x00400000		//����
#define MEDIA_FACTORYTYPE_FH			0x00800000		//���
#define MEDIA_FACTORYTYPE_HX			0x01000000		//����
#define MEDIA_FACTORYTYPE_ZB2			0x02000000		//����
#define MEDIA_FACTORYTYPE_H3C			0x04000000		//����
#define MEDIA_FACTORYTYPE_XC			0x08000000		//�Ų�
#define MEDIA_FACTORYTYPE_XW            0x10000000      //�������ϰ汾)
#define MEDIA_FACTORYTYPE_ZV            0x12000000      //�Ͼ���·
#define MEDIA_FACTORYTYPE_JL            0x20000000      //����
#define MEDIA_FACTORYTYPE_SR            0x40000000      //����
#define MEDIA_FACTORYTYPE_DB33V2		0x80000000		//DB33V2���г���

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
#define LOCAL_RECORD_STOP				101	//¼���Զ�ֹ֪ͣͨ
#define DECODE_FIRST_FRAME				102	//�յ��������һ֡��Ƶ
#define LOCAL_STREAM_STOP				105	//�յ��Է��Ľ���ͨ�������
#define LOCAL_STREAM_NULL				106	//����֡�ط�ģʽ�������յ�֪ͨ
#define LOCAL_FLAG_FRAME				107	//����֡�ط�ģʽ�⵽��ʶ֪֡ͨ

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


/* ͳ����Ϣ�ṹ */
typedef struct tagTCH_STAT_INFO
{
	UINT		unSize;						//���ṹ�峤��, sizeof
	UINT		unTimeStamp;				//ͳ��ʱ�̣�GetTickCount
	UINT		unTimeDiff;					//ͳ��ʱ��,ms
	UINT		unBitRate;					//������B/s
	UINT		unLostRate;					//�����ʣ�������ֱ�
	UINT		unFrameRate1;				//��ȡ��Ƶ֡֡�� fps
	UINT		unFrameRate2;				//�������ص�֡�� fps

	USHORT		usWidth;					// ��Ƶ���
	USHORT		usHeight;					// ��Ƶ�߶�

	int			nStartTime;					//ͨ����ʼʱ�䣬time-t
	int			nConnectTime;				//ͨ�����ӽ���ʱ�䣬time-t

	unsigned long long	u64RecvPacket1;		//��ͨ���������������յ��İ���Ŀ
	unsigned long long	u64RecvByte1;		//��ͨ���������������յ����ֽ���
	unsigned long long	u64RecvPacket2;		//��ͨ��������Э����յ�����Ч����Ŀ
	unsigned long long	u64RecvByte2;		//��ͨ��������Э����յ�����Ч�ֽ���

	UINT		unAudioType;				// ��Ƶ��������v2
	UINT		unVideoType;				// ��Ƶ��������v2����(MAKEFOURCC('S', 'V', 'M', '4')) ��ʾ ����mpeg4
	UCHAR		ucAudioType;				// ��Ƶ��������
	UCHAR		ucVideoType;				// ��Ƶ��������

	USHORT		usRealLocalPort;			//����ʹ�õ���ʵ�˿�
	USHORT		usLocalPort;				//���ذ󶨵Ķ˿�
	USHORT		usPeerPort;					//�Է��˿�
	char		szRealLocalAddr[64];		//������ʵ��ַ
	char		szLocalAddr[64];			//���ذ󶨵�ַ
	char		szPeerAddr[64];				//�Է���ַ

	char		szScokType[8];				//SOCKET ���� tcp,udp...

}TCH_STAT_INFO, *PTCH_STAT_INFO;

typedef struct tagConfigData
{
	char	szName[32];
	char	szValue[32];
}TCfgData, *LPTCfgData;

/* �汾��Ϣ�ṹ */
typedef struct
{
	unsigned	unMouduleLevel;		// ������ʾ������
	char			szModuleName[32];	//ģ����
	unsigned	unVer;				//0xyy.yy.yyyy�汾��
	int				nTime;				// compile time_t
	char			szVerInfo[128];
	char			szDescribe[256];
}MEDIA_VERSION_INFO;

typedef struct tagtFrameParam
{
	bool			bAgain;				//��������
	bool			bIsIFrame;			//�Ƿ�ΪI֡
	BYTE			byType;				//����:��Ƶ����Ƶ AUDIO_TYPE �� VIDEO_TYPE
	BYTE			byFrameRate;		//֡�� fps
	UINT			unCodecType;		//��������v2����(MAKEFOURCC('S', 'V', 'M', '4')) ��ʾ ����mpeg4
	USHORT			usWidth;			//ͼ����
	USHORT			usHeight;			//ͼ��߶�
	USHORT			usFactoryCode;		//���Ҵ���V2��0x686b��ʾ"hk"
	unsigned	unTimeStamp;		//32λʱ������λΪms����0��ʼ���ʱ��
	unsigned	dwFrameSeq;			//֡���кţ�ÿ֡��1
	unsigned long long	un64Timestamp;	//64λʱ������λΪms����19700101 00:00:00��ʼ���ʱ�䣬��Ҫ���ڻط�ʱ�������Ķ�λ
	unsigned	unOrgTimeStamp;		//�Ӱ�ͷ��ȡ����δ�޸ĵ�ʱ��
}TFrameParam;

/* ý������ṹ */
typedef struct tagTMediaParam
{
	bool			bAgain;				//��������
	BYTE			byType;				//����:��Ƶ����Ƶ AUDIO_TYPE �� VIDEO_TYPE
	BYTE			byEncodeType;		//�������ͣ����ֽڵ���������
	bool			bIsIFrame;			//�Ƿ�ΪI֡
	BYTE			byFrameRate;		//֡�� fps
	BYTE			byImageSize;		//ͼ��ֱ���
	unsigned	unTimeStamp;		//32λʱ������λΪms����0��ʼ���ʱ��
	unsigned	dwFrameSeq;			//֡���кţ�ÿ֡��1
}TMediaParam;

//�¼�֪ͨ�ص���¼����ֹͣ�������һ֡��ɵ��¼��ص���unEventIdΪ��ϢID���μ�notify event��pParam���û��þ�
typedef	int (__cdecl *MEDIA_NOTIFY_PROC)(unsigned unEventId, void* pParam);

//����ǰ������Ƶ�������ݻص���pBuf��֡���ݣ�nLen�����ݳ��ȣ�pFrameParam����ز�����pParam���û��þ�
typedef	int (__cdecl *MEDIA_FRAME_PROC)(unsigned char* pBuf, int nLen, TFrameParam* pFrameParam, void* pParam);

//��������Ƶ���ݻص���pBuf����Ƶ���ݣ�nLen�����ݳ��ȣ�nWidth����ȣ�nHeight���߶ȣ�
//nTime��ʱ���������ͬtime_t��nCSType����ɫ�ռ����� color space type��pParam���û��þ�
typedef	int (__cdecl *MEDIA_VIDEO_PROC)(unsigned char* pBuf, int nLen, int nWidth, int nHeight, int nTime, int nCSType, void* pParam);

//��������Ƶ���ݻص���pBuf����Ƶ���ݣ�nLen�����ݳ��ȣ�lpAudioFormat����Ƶ��ʽ����ΪWAVEFORMATEX*ʹ�ã�
//unTimeStamp��ʱ�������λms��pParam���û��þ�
typedef	int (__cdecl *MEDIA_AUDIO_PROC)(unsigned char* pBuf, int nLen, void* lpAudioFormat, unsigned unTimeStamp, void* pParam);

//��������Ƶ���ݻص���pBuf���������Ƶ���ݣ�nLen�����ݳ��ȣ�unCodecType����Ƶ��ʽ��unTimeStamp��ʱ�������λms��pParam���û��þ�
typedef	int (__cdecl *MEDIA_AUDIO_ENCODE_PROC)(unsigned char* pBuf, int nLen, unsigned unCodecType, unsigned unTimeStamp, void* pParam);

#ifdef __cplusplus
}
#endif	// __cplusplus

#endif	//__GE_COMMON_INCLUDE_H__
