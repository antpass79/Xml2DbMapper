using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Porting.Contract.Enums
{
	public enum ImageLayer
	{
		Annotations = 0,
		Measures,
		Ecg
	}

	public enum LogLevel
	{
		LogError = 0,
		LogNormal,
		LogInfo,
		LogDebug
	}

	public enum Flow_Type
	{
		Configuration = 0,
		Boot,
		Exam,
		UserCtrl,
		Warning,
		Archive,
		System,
		Dicom,
		ATest,
		RTChain,
		EchoAPI,
		UserSession,
		Unknown,
		Timeout
	}
	[DataContract(Name = "DateFormat")]
	public enum DateFormat
	{
		[EnumMember]
		GG_MM_AAAA = 0,
		[EnumMember]
		GG_MMM_AAAA,
		[EnumMember]
		MM_GG_AAAA,
		[EnumMember]
		MMM_GG_AAAA,
		[EnumMember]
		AAAA_MM_GG,
		[EnumMember]
		AAAA_MMM_GG,
		[EnumMember]
		Default
	};

	[DataContract(Name = "TimeFormat")]
	public enum TimeFormat
	{
		[EnumMember]
		HOURS_12 = 0,
		[EnumMember]
		HOURS_24,
		[EnumMember]
		Default
	};

	public enum DateTimeSize
	{
		DateTime,
		DateTimeNoSeconds,
		Date,
		DateNoYear,
		Time,
		TimeNoSeconds
	}


	public enum ViewerAreaEnum
	{
		A6,
		A7,
		A8a,
		A8b,
		A8c,
		A8d,
		//A9,   MyBug 16357
		//A10,  MyBug 16357
		A11u,
		A11d,
		A12,
		A13,
		A17
	}

	public enum ViewerLayoutEnum
	{
		Single,
		//$$RC Story 773
		SingleThermal,
		////$$RC Task 24047
		SingleThermalNoMeasure

	}

	public enum SystemEnvironment
	{
		Human = 0,
		Veterinary
	}

	public enum VNMeasureKey
	{
		Ruler = 0,
		Measure
	}
	public enum PhysicalModelBiolab
	{
		HwModel_6100,
		HwModel_6150,
		HwModel_6200,
		HwModel_6250,
		Desktop
	}
	public enum PhysicalModel
	{
		HwModel_6400,     // MyLab Seven
		HwModel_6480,     // Mylab Prototype (trolley)
		HwModel_6440,     // MyLab 9 (Lion)
		HwModel_7400,     // MyLab Alpha
		HwModel_7480,     // Mylab Prototype (portable)
		Desktop,          // MyLab Desk Evo
		HwModel_6420,     // MyLab Six
		HwModel_7430,     // MyLab Delta
		HwModel_7410,     // MyLab Gamma
		HwModel_6425,     // MyLab Six Cristalline (CL)
		HwModel_6450,     // MyLab X8 (Black Panther)
		HwModel_6400_V3,  // MyLab X6 and MyLab X7
		HwModel_7400_V3,  // MyLab Omega
		HwModel_6420_V3,  // MyLab X5
		HwModel_7410_V3,  // MyLab Sigma
		HwModel_7430_V3,  // MyLab Delta V3
		HwModel_All,      // Used to manage common features ( Keep this always as first after real models )
		HwModel_None,     // Used to manage common features
	}

	public enum PhysicalSubModel
	{
		Model_00,
		Model_01,
		Model_02,
		Model_03,
		Model_04
	}

	public enum MonitorAutodetectionStatus
	{
		ProperlyDetected,
		NotDetected,
		NotSupported,
		CommunicationProblem,
		Skipped,
		Disabled
	}

	public enum SortingDirection
	{
		Ascending = 0,
		Descending = 1
	}

	public enum ArchiveFilterOperator
	{
		Equal = 0,
		Like,
		NotMinor,
		NotGreater,

		/// <summary>
		/// This enum identify a sql command not to be changed
		/// </summary>
		SqlCommand
	}

	public enum Sex
	{
		Sex_Unknown = 0,
		Sex_Male,
		Sex_Female,
		Sex_Other
	};

	public enum FlowButtons
	{
		None,
		OK_Cancel,
		OK,
		Yes_No,
		Cancel,
		Close,
		Yes_No_Cancel,
		Locked,
		Save_Cancel,
		OK_Print_Cancel,
		Exit,
		OnlyTab,
		Yes_Cancel
	}
	//$$RC  Task 2960
	public enum ModalDialogTermination
	{
		OK,
		Cancel,
		Yes,
		NO,
		Close,
		ProgressBarClosed,
		Menubutton,
		PrintNow //$$RC   Task 2960

	}

	public enum StudyFormat
	{
		Native = 0,
		Dicom
	}

	public enum ImageFormat
	{
		Native = StudyFormat.Native,
		Dicom = StudyFormat.Dicom
	}

	// Formato di sonda
	public enum eTsdFormat
	{
		eConvex = 0,            // formato Convex
		eLinear,                // formato Lineare
		ePencil,                // formato Pencil
	}

	public enum ApplicationType
	{
		NoType = -1,
		Cardiac = 0,
		Vascular = 1,
		Abdominal = 2,
		Obstetric = 3,
		Transcranic = 4,
		Neonatal = 5,
		CardiacPediatric = 6,
		Pediatric = 7,
		Gynecology = 8,
		Urologic = 9,
		MusculoSkeletal = 10,
		Breast = 11,
		Thyroid = 12,
		SmallParts = 13,
		Generic = 14,
		AbdominalCanine = 15,
		AbdominalFeline = 16,
		AbdominalEquine = 17,
		ReproductionCanine = 18,
		ReproductionFeline = 19,
		ReproductionEquine = 20,
		Tendon = 21,
		CardioCanine = 22,
		CardioFeline = 23,
		CardioEquine = 24,
		OvaryEquine = 25,
		OvaryBovine = 26,
		ReproductionBovine = 27,
		VascularVet = 28,
		AnimalScience = 29,
		ReproductionOvine = 30,
		ReproductionPorcine = 31,
		ReproductionCaprine = 32,
		RegAnest = 33,
		Aesthetic = 34,
		OtherAbdominal = 35,
		OtherCardio = 36,
		OtherVascular = 37,
		OtherReproduction = 38,
		AbdominalBovine = 39,
		CardioBovine = 40,
		GeneralImaging = 41,
		GeneralImagingVet = 42,
		ObGyn = 43,
		MKSVet = 44,
		obgynVet = 45,
		CardioVet = 46,
		CardioHuman = 47,
		VascularHuman = 48,
		Ophtalmic = 49,
		SharedServicesGVW = 50,
		SharedServicesGVC = 51,
		CardioPack = 52,
		CardioBundle = 53,
		VascularPack = 54,
		VascularBundle = 55
	}

	public enum VortexCompatibilityIssue
	{
		GENERIC_ERROR,
		NON_CARDIO,
		NON_VORTEXDATA,
		TOO_MANY_FRAMES,
		NO_SINGLE_B_CALIBRATION_REGION,
		INVALID_MASK_VALUE,
		INVALID_VELOCITY_VALUE,
		INVALID_CFM_BOX_LOCATION,
		INCOMPATIBLE_MASK_VS_VELOCITY_SIZE,
		INVALID_PROBE_CENTRE_OF_CURVATURE,
		INVALID_SUBSAMPLING,
		INVALID_CFM_SCALE_FACTOR,
		NON_CALIBRATIONREGIONSHOMOGENEOUS
	}

	public enum XStrainCompatibilityIssue
	{
		NON_MULTIFRAME,
		NO_HEARTRATE,
		NO_TRIGGERECG,
		NON_CARDIO,
		NON_STANDARD,
		NOT_HIGHFRAMERATE,
		NON_NATIVE,
		NON_CALIBRATIONREGIONSHOMOGENEOUS,
		GENERIC_ERROR,
		TOO_MANY_CYCLES,
		NOCALIBRATIONREGIONS,
		ISZOOM,
		IS73xx,
		Is61xx_Bef1001,
		Is61xxMissingSoftwareVersion,
		ISPROSPECTIVE,
		DEP,
		OVERHIGHFRAMERATE,
		NON_PHASEARRAY
	}

	public enum DicomImportIssue
	{
		UNDEFINED = 0,
		IsStructuredReport,
		IsNotEsaoteImage,
		IsNotImportableImage
	}


	// Enumerates the available exam types
	public enum ExamType
	{
		OTHER,
		STANDARD,
		WALL_MOTION,
		CNTI,
		DDD_BSCAN,
		VPAN,
		DDD_FREE_HAND,
		DDD_TRACKER,
		QIMT,
		ELASTO,
		NAVIGATOR,
		DDD_STIC,
		UNKNOWN //leave this item at the end of enum
	}

	// Enumerates the available age's unit type
	public enum AgeUnit
	{
		Days,
		Weeks,
		Months,
		Years
	}

	// Enumerates the available image types
	public enum ImageType
	{
		SINGLE_FRAME,
		MULTI_FRAME,
		DDD_IMAGE
	}

	// Enumerates the available types of vectorial data
	public enum VectorialDataType
	{
		OVERLAY,
		ECG_TRACE,
		INFO_AREA
	}

	// Enumerates the possible vectorial layers
	public enum LayerType
	{
		GENERIC = 0x0001,
		ANAGRAPHIC = 0x0002,
		MEASURES = 0x0004,
		ANNOTATIONS = 0x0008,
		ECG_TRACE = 0x0010,
		BODY_MARKS = 0x0020
	}

	// Enumerates the available image format types
	public enum ImageFormatType
	{
		FULL,
		DUAL,
		QUAD,
		ESADEC,
		OTHER_FORMAT
	}

	// Enumerates the available probe types
	public enum ProbeType
	{
		UNKNOWN,
		LINEAR,
		CONVEX,
		PHASE_ARRAY,
		PENCIL
	}

	// Gibiino
	public enum TransducerPosition
	{
		Frontal,
		Lateral
	}


	// Enumerates the available content data types
	public enum DataType
	{
		IMAGES,
		REPORT,
		PROBE_DATA
	}

	// Used to distinguish restore or save Measure.xml report file
	public enum ReportActivity
	{
		Restore,
		Export,
	}

	// Return codes when trying to restore or save Measure.xml report file
	public enum ReportManagementCode
	{
		NoError,
		InvalidValue,             //Element with invalid value
		MemoryAllocationError,    //Memory allocation problem
		GenericError,             //Generic error
		FileError,                //File error
		FatalError,               //Fatal error: shutdown required
		BadFileError,             //XML error on read
		UnknownVersionFileError,  //Unsupported XML file version
		MissingFileError          //Missing XML file
	}

	// Enumerates the available measurement units
	public enum MeasureUnit
	{
		NotAvailable,
		Percentual,
		dB,
		dB_per_sec,
		Meter,
		Feet,
		Centimeter,
		Millimeter,
		SquareMeter,
		SquareCentimeter,
		SquareMillimeter,
		Second,
		Millisecond,
		kHz,
		Hz,
		Degree,
		Kg,
		Lbs,
		Gram,
		NanoGram,
		MilliLiter,
		CC,
		CubicCentimeter,
		Meter_per_sec,
		Centimeter_per_sec,
		Millimeter_per_sec,
		SquareCentimeter_per_sec,
		CubicCentimeter_per_sec,
		Liter_per_minute,
		Milliliter_per_minute,
		Milliliter_per_sec,
		kHz_per_Centimeter_per_sec,
		kHz_per_Meter_per_sec,
		Hertz_per_Meter_per_sec,
		Meter_per_SquareSec,
		Centimeter_per_SquareSec,
		Liter_per_minute_per_SquareMeter,
		Gram_per_SquareMeter,
		Bmp,
		NanoGram_per_MilliLiter,
		MillimeterHg,
		MillimeterHg_per_sec,
		Date,
		Years,
		Months,
		Weeks,
		Days,
		Millimeter_per_SquareMeter,
		Milliliter_per_SquareMeter,
		kPa,
		SecondAtMinusOne,
		Micrometer,
		Milligram_per_Deciliter,
		Degree_per_second,
		kPaAtMinusOne,
		SquareMillimeter_per_KiloPascal,
		NanoGram_per_MilliLiter_per_CC,
		Adimensional,
		CubicCentimeter_per_SquareMeter,
		SquareCentimeter_per_SquareMeter,
		Millimeter_per_minute,
	}

	// Enumerates the available photo interpretation types
	public enum PhotoInterpretationType
	{
		PALETTE_COLOR,
		RGB,
		YBR_FULL,
		YBR_FULL_422,
		PALETTE_ALPHA,
		BGRA,
		RGBA,
		UNKNOWN_INTERP
	}

	// Enumerates the available planar configurations
	public enum PlanarConfigurationType
	{
		eColourByPixel,   // Data are saved pixel by pixel
		eColourByPlane    // Data are saved plane by plane
	}

	// Enumerates the available compression algorithms
	public enum CompressionAlgType
	{
		NoCompression,
		RLE,
		JPEG_BASELINE,
		JPEG_EXTENDED_PROCESS2,
		JPEG_EXTENDED_PROCESS4,
		JPEG_LOSSLESS,
		JPEG_2000,
		JPEG_LS,
		RLE_INTEL
	}

	// Enumerates the stress protocol types
	public enum StressProtocolType
	{
		RETROSPECTIVE,     // RETROSPECTIVE
		PROSPECTIVE,       // PROSPECTIVE
		MONITORING,        // MONITORING
		FOLLOW_UP,         // FOLLOW-UP
		CONTINUOUS_CAPTURE // Continuous Capture
	}

	// Enumerates the EDD/GA/LMP presence on screen
	public enum OBCaptionInfoType
	{
		GAOnCaption,     // GA on screen
		EDDOnCaption,       // EDD on screen
		LMPOnCaption,        // LMP on screen
	}

	// Enumerates the cardiac projection types
	public enum CardioProjectionType
	{
		CUSTOM,   // User defined or unknown
		LAX,      // Parasternal Long Axis
		SAX_PM,   // Short Axis Papillary Muscle
		A4C,      // Apical Four Chamber
		A2C,      // Apical Two Chamber
		SAX_MV,   // Short Axis Mitral Valve
		SAX_AP,   // Short Axis Apex
		ALAX      // Apical Long Axis
	}

	public enum PostMenoPause
	{
		Undefined,
		Yes,
		No
	}

	public enum ObstetricMode
	{
		FetalAge,
		Fetalgrowth
	}

	public enum DGA
	{
		FDGA,
		LmpEdd,
		DOC
	}

	public enum AnimalSpecies
	{
		Canine = 0,
		Feline,
		Equine,
		Bovine,
		Ovine,
		Caprine,
		Porcine,
		Other,
		Unknown
	}

	public enum SexNeutered
	{
		No = 0,
		Yes,
		Unknown
	}

	public enum MeausureUnitMetric
	{
		International_System = 0,
		Anglo_Saxon
	}

	public enum ExportFormat
	{
		Dicom = 0,
		Multimedia,
		Native,
		Unknown
	}

	public enum TouchScreenMenuType
	{
		Small, //tipo bodymarks
		Normal //tipo misure
	}

	public enum UserLevel
	{
		Unknown = -1,
		RS = 0,
		Service,
		Standard, // cioè ecouser
		Test,     // cioe' collaudo
		WIP
	}

	public enum EcoMode
	{
		Invalid = -1,
		B_Mode_MS = 0,
		M_Mode_MS,
		PW_Mode_MS,
		B_Mode_CFM,
		CW_Mode_MS
	}

	public enum SpeciesTypes
	{
		eUNKNOWN = 0,
		eCANINE,
		eFELINE,
		eEQUINE,
		eBOVINE,
		eOVINE,
		eCAPRINE,
		ePORCINE,
		eOTHER,
		eLAST_SPECIES_TYPE = eOTHER
	}

	public enum TouchScreenModality
	{
		Primary,
		Secondary,
		InMain
	}

	public enum TsArea
	{
		MainButtonsArea,
		SpecialButtonsArea,
		TogglesArea
	}

	public enum TsTemplates
	{
		Standard,
		Standard2,
		StandardLion,
		StandardLion2,
		StandardLion3,
		StandardLion4,
		StandardLion5,
		StandardLion6,
		Menu,
		MenuLarge,
		MenuLion,
		MultiColumn,
		MultiColumnLion,
		Sliders,
		SlidersLion,
		WorksheetGraphHighEquipmentTemplate,
		WorksheetGraphAllEquipmentTemplate,
		AdvancedLibraryHighEquipmentTemplate,
		AdvancedLibraryAllEquipmentTemplate,
		Invalid
	}

	public enum HRStatus
	{
		Invalid = 0,
		Valid,
		ToBig,
		ToLow
	}

	public enum BSAType
	{
		Standard = 0,  //adult
		Pediatric,
		Custom
	}

	public enum ADMProfileType
	{
		Full = 0,
		Positive,
		Negative
	}

	public enum ADMFlowProfileType
	{
		Peak = 0,
		Mean,
		Mode
	}


	public enum ElementOnViewerInitialPosition
	{
		Left = 0,
		Right
	}


	public enum SettableCollectionType
	{
		System = 0,
		Clinical,
		Model,
		None
	}

	public enum MediaInfoStatus
	{
		UsbNotPresent = 0,
		UsbNotWritable,
		UsbNotReady,
		UsbFull,
		UsbNotEnoughSpace,
		BurnerDeviceNotConnected,
		CDNotPresent, //for CD and DVD
		CDNotWritable, //for CD and DVD
		CDNotReady, //for CD and DVD
		CDNotEmpty, //for CD and DVD
		CDNotEnoughSpace,
		CD_BurnNotPossible, //for example if nero version is not 7.5
		CD_OnBattery, //Avoid to burn DVD if running on battery
		HDNotPresent,
		HDNotWritable,
		HDNotReady,
		HDFull,
		HDNotEnoughSpace,
		NetDirNotConnected,
		NetDirNotWritable,
		NetDirNotReady,
		NetDirFull,
		NetDirNotEnoughSpace,
		NoProblem
	}

	//ogni item del seguente enum è in corrispondenza con un campo di input di patient data o un campo del report (per es. LMP(DGA))
	public enum PatientDataField
	{
		LASTNAME,
		FIRSTNAME,
		MIDDLENAME,
		REFERRINGPHYSICIAN,
		PERFORMINGPHYSICIAN,
		OPERATOR,
		ID,
		BIRTHDATE,
		AGE,
		GENDER,
		DIAGNOSIS,
		ACCESSION,
		HEIGHT,
		WEIGHT,
		BODY_SURFACE_AREA,
		SYSTOLIC_BLOOD_PRESSURE,
		DYASTOLIC_BLOOD_PRESSURE,
		PSA,
		LAST_MENSTRUAL_PERIOD,
		DATE_OF_CONCEPTION,
		EXPECTED_DELIVERY_DATE,
		FirstDGAWeeks,
		FirstDGADays,
		FIRST_DGA_DATE,
		DGAWeeks,
		DGADays,
		GRAVIDA,
		PARA,
		ABORTA,
		ECTOPIC,
		EDD_BY_LMP,
		EDD_BY_FDGA,
		DGA_BY_LMP,
		DGA_BY_FDGA,
		AGE_UNIT,
		Last_First_Middle_more_than_60_char,
		ReferringPhysician_Last,
		ReferringPhysician_First,
		ReferringPhysician_Middle,
		ReferringPhysician_Prefix,
		ReferringPhysician_Suffix,
		ReferringPhysician_more_than_60_char,
		PerformingPhysician_Last,
		PerformingPhysician_First,
		PerformingPhysician_Middle,
		PerformingPhysician_Prefix,
		PerformingPhysician_Suffix,
		PerformingPhysician_more_than_60_char,
		Operator_Last,
		Operator_First,
		Operator_Middle,
		Operator_Prefix,
		Operator_Suffix,
		Operator_more_than_60_char,
		Studydate_not_present,
		DGA_byLmp__More_than_44_weeks,
		DGA_byFDGA__More_than_44_weeks,
		Doc_Older_Than_44Weeks,
		SomeFDGAFieldNotPresentOrNotValid,
		STUDYDESCRIPTION,
		POST_MENOPAUSE,
		CYCLE_DAYS,
		FIRST_DGA,
		EDD,
		DGA,
		EXAM_DATE,
		REPORT_DATE,

		//VET
		Owner_AnimalName_more_than_62_char,
		OWNER_LAST_NAME,
		ANIMAL_NAME,
		BREED,
		NEUTERED
	}

	public enum DICOMServerErrorType
	{
		NotValid = 0,
		Completed,
		AssociationError,
		TransmissionError,
		UnknownError    // Gibiino [RTC 11931] unknown error added for generic failure or exception
	}

	public enum DICOM_SinkReturnValues
	{
		NotValid,
		AbortGeneric,
		Retry,
		Continue,
		AbortNoMoreSpace,
		AbortScpNotEnabled
	}

	public enum MPPSType
	{
		Start = 0,
		Completed,
		Discontinued
	}

	public enum UsersType
	{
		Normal = 0,
		Emergency,
		Esaote,
		Administrator,
		UsersDisabled,
		NoValidUser,
		NormalNoMenu
	}

	public enum DICOM_ReportExport
	{
		None = 0,
		SecondaryCapture,
		StructuredReport,
		ExportToEstensa
	}

	// Enumerates some commands necessary to understand a IGDSLayer
	public enum LayerCommand
	{
		Unknown,
		LayerSize,
		Text,
		Line,
		Move
	}

	// Enumerates laterality in measurements (i.e. vascular application)
	public enum Laterality
	{
		UNILATERAL = 0,
		LEFT,
		RIGHT
	}

	// Enumerates the different DICOM temporary directory types
	public enum DicomOperationsType
	{
		NotDefined = 0,
		CD_DVD_Burn,
		StorageSCU,
		Report,
		PrintSCU,
		Crypto
	}
	// Enumerates types of float numeric value
	public enum FloatState
	{
		FVALID,
		OUTOFRANGE,
		FNULL,
		TOOBIG,
		INVALID
	}

	public enum CharacterSets
	{
		Latin,
		Cyrillic,
		SimplifiedHanzi,
		UTF8,
		Latin2
	}

	public enum PatientIDRequestFor
	{
		StartExam = 0,
		ModifyPatientData
	}

	// Cfm Mix
	public enum eCfmMix
	{
		eCfmVelMix,
		eCfmPowMix,
		eCfmVarMix,
		eCfmVelVarMix,
		eCfmVelPowMix,
		eCfmVarPowMix,
		eCfmSigPowMix,
		eCfmElastoMix,
		eCfmOnly
	}

	public enum WindowAppearanceMode
	{
		Unknown = -1,
		Default = 0,
		FullHd = 1,
		Center = 2,
		Custom = 3,
		HDReady = 4
	}

	public enum ScreenResolution
	{
		Screen_1280x720 = 0,
		Screen_1280x768,
		Screen_1280x800,
		Screen_1366x768,
		Screen_1920x1080,
		Screen_1920x1200
	}

	public enum RT_ToolID
	{
		None = 0x0000,
		VPan = 0x0001,
		QImt = 0x0002,
		MyLib = 0x0004,
		ThreeD = 0x0008,
		FourD = 0x0010,
		Stress = 0x0020,
		CNTI = 0x0040,
		Elasto = 0x0080
	}

	// 2D Format
	public enum e2DFormat
	{
		eCenter = 0,      // formato dual Centrato
		eLeft,            // formato dual Sinistro
		eRight,           // formato dual Destro
		eLeftUp,          // Quad LR
		eRightUp,         // Quad RL
		eLeftDown,        // Quad LD
		eRightDown,       // Quad RD
	}

	public enum ExecuteCommandResult
	{
		OK, NotFound, NotEnabled, TimedOut
	}

	public enum ObjOnViewerBorderPosition
	{
		Left = 0,
		Right,
		Top,
		Bottom,
		None
	}

	/// <summary>
	/// Files used by Navigator
	/// </summary>
	public enum NavigatorFileType
	{
		UnknownType = -1,
		NoNavigator,
		Config,
		Suite3D,
		BreastConfig
	}

	public class Enums
	{
		public static bool IsCardiacApplication(ApplicationType p_Application)
		{
			switch (p_Application)
			{
				case ApplicationType.Cardiac:
				case ApplicationType.CardiacPediatric:
				case ApplicationType.CardioCanine:
				case ApplicationType.CardioEquine:
				case ApplicationType.CardioFeline:
				case ApplicationType.OtherCardio:
					return true;

				default:
					break;
			}
			return false;
		}

		static public bool IsApplicationCardiac(ApplicationType p_Application)
		{
			if (p_Application == ApplicationType.Cardiac ||
					p_Application == ApplicationType.CardiacPediatric ||
					p_Application == ApplicationType.CardioCanine ||
					p_Application == ApplicationType.CardioFeline ||
					p_Application == ApplicationType.CardioEquine ||
					p_Application == ApplicationType.CardioBovine ||
					p_Application == ApplicationType.OtherCardio)
			{
				return true;
			}

			return false;
		}

	}
}
