"""Key for all IGDB platform IDs.
See: https://api-docs.igdb.com/#platform
"""
from enum import IntEnum


class IgdbPlatforms(IntEnum):
    """IGDB platform Id table for API requests"""
    # PlayStation
    PS1 = 7
    PS2 = 8
    PS3 = 9
    PS4 = 48
    PS5 = 167
    PSP = 38
    PS_VITA = 46
    PS_VR = 165

    # Xbox
    XBOX = 11
    XBOX_360 = 12
    XBOX_ONE = 49
    XBOX_SERIES = 169
    XBOX_LIVE_ARCADE = 36

    # Nintendo
    NES = 18
    SNES = 19
    N64 = 4
    GAMECUBE = 21
    WII = 5
    WII_U = 41
    SWITCH = 130
    GAME_BOY = 33
    GAME_BOY_COLOR = 22
    GAME_BOY_ADVANCE = 24
    DS = 20
    N3DS = 37
    VIRTUAL_BOY = 87
    FAMICOM_DISK_SYSTEM = 51
    GAME_AND_WATCH = 153
    POKEMON_MINI = 166

    # Sega
    MASTER_SYSTEM = 64
    GENESIS = 29
    SEGA_32X = 30
    SEGA_CD = 78
    SATURN = 32
    DREAMCAST = 23
    GAME_GEAR = 35
    SG_1000 = 84

    # PC
    PC = 6
    LINUX = 3
    MAC = 14
    DOS = 13
    WEB_BROWSER = 82

    # Mobile
    ANDROID = 34
    IOS = 39

    # Atari
    ATARI_2600 = 59
    ATARI_5200 = 66
    ATARI_7800 = 60
    ATARI_LYNX = 61
    ATARI_JAGUAR = 62
    ATARI_ST = 63
    ATARI_8BIT = 65

    # NEC
    TURBOGRAFX_16 = 86
    PC_ENGINE_CD = 149
    PC_ENGINE_SUPERGRAFX = 138
    PC_FX = 123

    # SNK
    NEO_GEO_MVS = 79
    NEO_GEO_AES = 80
    NEO_GEO_POCKET = 119
    NEO_GEO_POCKET_COLOR = 120

    # Commodore
    COMMODORE_VIC20 = 71
    COMMODORE_64 = 15
    COMMODORE_16 = 134
    COMMODORE_PLUS4 = 94
    COMMODORE_PET = 90
    AMIGA = 114
    AMIGA_CD32 = 115
    COMMODORE_CDTV = 158

    # Other
    ARCADE = 52
    THREE_DO = 122
    INTELLIVISION = 67
    COLECOVISION = 68
    VECTREX = 70
    WONDERSWAN = 57
    WONDERSWAN_COLOR = 124
    N_GAGE = 135
    OUYA = 72
    STADIA = 170
    OCULUS_VR = 162
    PLAYDATE = 160
