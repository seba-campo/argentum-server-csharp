namespace Argentum.Network.Packets;

/// <summary>
/// Identificadores de paquetes enviados desde el Servidor hacia el Cliente.
/// Tamaño de red: 2 bytes (Int16 / short).
/// </summary>
public enum ServerPacketId : short
{
    eMinPacket,
    eConnected,
    elogged, // LOGGED  0
    eRemoveDialogs, // QTDL
    eRemoveCharDialog, // QDL
    eNavigateToggle, // NAVEG
    eEquiteToggle,
    eDisconnect, // FINOK
    eCommerceEnd, // FINCOMOK
    eBankEnd, // FINBANOK
    eCommerceInit, // INITCOM
    eBankInit, // INITBANCO
    eUserCommerceInit, // INITCOMUSU   10
    eUserCommerceEnd, // FINCOMUSUOK
    eShowBlacksmithForm, // SFH
    eShowCarpenterForm, // SFC
    eNPCKillUser, // 6
    eBlockedWithShieldUser, // 7
    eBlockedWithShieldOther, // 8
    eCharSwing, // U1
    eSafeModeOn, // SEGON
    eSafeModeOff, // SEGOFF 20
    ePartySafeOn,
    ePartySafeOff,
    eCantUseWhileMeditating, // M!
    eUpdateSta, // ASS
    eUpdateMana, // ASM
    eUpdateHP, // ASH
    eUpdateGold, // ASG
    eUpdateExp, // ASE 30
    eChangeMap, // CM
    ePosUpdate, // PU
    eNPCHitUser, // N2
    eUserHittedByUser, // N4
    eUserHittedUser, // N5
    eChatOverHead, // ||
    eLocaleChatOverHead,
    eConsoleMsg, // || - Beware!! its the same as above, but it was properly splitted
    eConsoleFactionMessage,
    eGuildChat, // |+   40
    eShowMessageBox, // !!
    eMostrarCuenta,
    eCharacterCreate, // CC
    eCharacterRemove, // BP
    eCharacterMove, // MP, +, * and _
    eCharacterTranslate,
    eUserIndexInServer, // IU
    eUserCharIndexInServer, // IP
    eForceCharMove,
    eCharacterChange, // CP
    eObjectCreate, // HO
    efxpiso,
    eObjectDelete, // BO  50
    eBlockPosition, // BQ
    ePlayMIDI, // TM
    ePlayWave, // TW
    eguildList, // GL
    eAreaChanged, // CA
    ePauseToggle, // BKW
    eRainToggle, // LLU
    eCreateFX, // CFX
    eUpdateUserStats, // EST
    eWorkRequestTarget, // T01 60
    eChangeInventorySlot, // CSI
    eInventoryUnlockSlots,
    eChangeBankSlot, // SBO
    eChangeSpellSlot, // SHS
    eAtributes, // ATR
    eBlacksmithWeapons, // LAH
    eBlacksmithArmors, // LAR
    eBlacksmithExtraObjects,
    eCarpenterObjects, // OBR
    eRestOK, // DOK
    eErrorMsg, // ERR
    eBlind, // CEGU 70
    eDumb, // DUMB
    eShowSignal, // MCAR
    eChangeNPCInventorySlot, // NPCI
    eUpdateHungerAndThirst, // EHYS
    eMiniStats, // MEST
    eLevelUp, // SUNI
    eAddForumMsg, // FMSG
    eShowForumForm, // MFOR
    eSetInvisible, // NOVER 80
    eMeditateToggle, // MEDOK
    eBlindNoMore, // NSEGUE
    eDumbNoMore, // NESTUP
    eSendSkills, // SKILLS
    eTrainerCreatureList, // LSTCRI
    eguildNews, // GUILDNE
    eOfferDetails, // PEACEDE & ALLIEDE
    eAlianceProposalsList, // ALLIEPR
    ePeaceProposalsList, // PEACEPR 90
    eCharacterInfo, // CHRINFO
    eGuildLeaderInfo, // LEADERI
    eGuildDetails, // CLANDET
    eShowGuildFundationForm, // SHOWFUN
    eParalizeOK, // PARADOK
    eStunStart, // Stun start time
    eShowUserRequest, // PETICIO
    eChangeUserTradeSlot, // COMUSUINV
    eUpdateTagAndStatus,
    eFYA,
    eCerrarleCliente,
    eContadores,
    eShowPapiro,
    eUpdateCooldownType,
    eSpawnListt, // SPL
    eShowSOSForm, // MSOS
    eShowMOTDEditionForm, // ZMOTD
    eShowGMPanelForm, // ABPANEL
    eUserNameList, // LISTUSU
    eUserOnline, // 110
    eParticleFX,
    eParticleFXToFloor,
    eParticleFXWithDestino,
    eParticleFXWithDestinoXY,
    ehora,
    eLight,
    eAuraToChar,
    eSpeedToChar,
    eLightToFloor,
    eNieveToggle,
    eNieblaToggle,
    eGoliath,
    eTextOverChar,
    eTextOverTile,
    eTextCharDrop,
    eConsoleCharText,
    eFlashScreen,
    eAlquimistaObj,
    eShowAlquimiaForm,
    eSastreObj,
    eShowSastreForm, // 126
    eVelocidadToggle,
    eMacroTrabajoToggle,
    eBindKeys,
    eShowFrmLogear,
    eShowFrmMapa,
    eInmovilizadoOK,
    eBarFx,
    eLocaleMsg,
    eShowPregunta,
    eDatosGrupo,
    eubicacion,
    eArmaMov,
    eEscudoMov,
    eViajarForm,
    eNadarToggle,
    eShowFundarClanForm,
    eCharUpdateHP,
    eCharUpdateMAN,
    ePosLLamadaDeClan,
    eQuestDetails,
    eQuestListSend,
    eNpcQuestListSend,
    eUpdateNPCSimbolo,
    eClanSeguro,
    eIntervals,
    eUpdateUserKey,
    eUpdateRM,
    eUpdateDM,
    eSeguroResu,
    eLegionarySecure,
    eStopped,
    eInvasionInfo,
    eCommerceRecieveChatMessage,
    eDoAnimation,
    eOpenCrafting,
    eCraftingItem,
    eCraftingCatalyst,
    eCraftingResult,
    eGuardNotice,
    eAnswerReset,
    eObjQuestListSend,
    eUpdateBankGld,
    ePelearConPezEspecial,
    ePrivilegios,
    eShopInit,
    eUpdateShopClienteCredits,
    eSendSkillCdUpdate,
    eUpdateFlag,
    eCharAtaca,
    eNotificarClienteSeguido,
    eGetInventarioHechizos,
    eNotificarClienteCasteo,
    ePosUpdateUserChar,
    ePosUpdateChar,
    ePlayWaveStep,
    eShopPjsInit,
    eDebugDataResponse,
    eCreateProjectile,
    eUpdateTrap,
    eUpdateGroupInfo,
    eUpdateCharValue, // updates some char index value based on enum
    eSendClientToggles, // Get active feature Toggles from server
    eAntiCheatMessage,
    eAntiCheatStartSession,
    eReportLobbyList,
    eAccountCharacterList,
    eChangeSkinSlot,
    eGuildConfig,
    eShowPickUpObj,
    eRemortState,
    eRemortResult,
    eHooTargetedSpellCastResult,
    eHooHouseDoorActionResult,
    eMaxPacket,
}

