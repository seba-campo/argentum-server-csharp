namespace Argentum.Network.Packets;

/// <summary>
/// Identificadores de paquetes enviados desde el Cliente hacia el Servidor.
/// Tamaño de red: 2 bytes (Int16 / short).
/// </summary>
public enum ClientPacketId : short
{
    eMinPacket,
    eCraftCarpenter, // CNC
    eWorkLeftClick, // WLC
    eCreateNewGuild, // CIG
    eSpellInfo, // INFS
    eEquipItem, // EQUI
    eChangeHeading, // CHEA
    eModifySkills, // SKSE
    eTrain, // ENTR
    eCommerceBuy, // COMP
    eBankExtractItem, // RETI
    eCommerceSell, // VEND
    eBankDeposit, // DEPO
    eForumPost, // DEMSG
    eMoveSpell, // DESPHE
    eClanCodexUpdate, // DESCOD
    eUserCommerceOffer, // OFRECER
    eGuildAcceptPeace, // ACEPPEAT
    eGuildRejectAlliance, // RECPALIA
    eGuildRejectPeace, // RECPPEAT
    eGuildAcceptAlliance, // ACEPALIA
    eGuildOfferPeace, // PEACEOFF
    eGuildOfferAlliance, // ALLIEOFF
    eGuildAllianceDetails, // ALLIEDET
    eGuildPeaceDetails, // PEACEDET
    eGuildRequestJoinerInfo, // ENVCOMEN
    eGuildAlliancePropList, // ENVALPRO
    eGuildPeacePropList, // ENVPROPP
    eGuildDeclareWar, // DECGUERR
    eGuildNewWebsite, // NEWWEBSI
    eGuildAcceptNewMember, // ACEPTARI
    eGuildRejectNewMember, // RECHAZAR
    eGuildKickMember, // ECHARCLA
    eGuildUpdateNews, // ACTGNEWS
    eGuildMemberInfo, // 1HRINFO<
    eGuildOpenElections, // ABREELEC
    eGuildRequestMembership, // SOLICITUD
    eGuildRequestDetails, // CLANDETAILS
    eOnline, // /ONLINE
    eQuit, // /SALIR
    eGuildLeave, // /SALIRCLAN
    eRequestAccountState, // /BALANCE
    ePetStand, // /QUIETO
    ePetFollow, // /ACOMPA�AR
    ePetLeave, // /LIBERAR
    eGrupoMsg, // /GrupoMsg
    eTrainList, // /ENTRENAR
    eRest, // /DESCANSAR
    eMeditate, // /MEDITAR
    eResucitate, // /RESUCITAR
    eHeal, // /CURAR
    eHelp, // /AYUDA
    eRequestStats, // /EST
    eCommerceStart, // /COMERCIAR
    eBankStart, // /BOVEDA
    eInformation, // /INFORMACION
    eReward, // /RECOMPENSA
    eRequestMOTD, // /MOTD
    eUpTime, // /UPTIME
    eGuildMessage, // /CMSG
    eGuildOnline, // /ONLINECLAN
    eCouncilMessage, // /BMSG
    eFactionMessage, // /FMSG
    eRoleMasterRequest, // /ROL
    eChangeDescription, // /DESC
    eGuildVote, // /VOTO
    epunishments, // /PENAS
    eGamble, // /APOSTAR
    eMapPriceEntrance, // /ARENA
    eLeaveFaction, // /RETIRARFACCION
    eBankExtractGold, // /RETIRAR ( with arguments )
    eBankDepositGold, // /DEPOSITAR
    eDenounce, // /DENUNCIAR
    eLoginExistingChar, // OLOGIN
    eLoginNewChar, // NLOGIN
    eTalk, // ;
    eYell, // -
    eWhisper, // \
    eWalk, // M
    eRequestPositionUpdate, // RPU
    eAttack, // AT
    ePickUp, // AG
    eSafeToggle, // /SEG & SEG  (SEG
    ePartySafeToggle,
    eRequestGuildLeaderInfo, // GLINFO
    eRequestAtributes, // ATR
    eRequestSkills, // ESKI
    eRequestMiniStats, // FEST
    eCommerceEnd, // FINCOM
    eUserCommerceEnd, // FINCOMUSU
    eBankEnd, // FINBAN
    eUserCommerceOk, // COMUSUOK
    eUserCommerceReject, // COMUSUNO
    eDrop, // TI
    eCastSpell, // LH
    eLeftClick, // LC
    eDoubleClick, // RC
    eWork, // UK
    eUseSpellMacro, // UMH
    eUseItem, // USA
    eCraftBlacksmith, // CNS
    eGMMessage, // /GMSG
    eshowName, // /SHOWNAME
    eOnlineRoyalArmy, // /ONLINEREAL
    eOnlineChaosLegion, // /ONLINECAOS
    eGoNearby, // /IRCERCA
    ecomment, // /REM
    eWhere, // /DONDE
    eCreaturesInMap, // /NENE
    eWarpMeToTarget, // /TELEPLOC
    eWarpChar, // /TELEP
    eSilence, // /SILENCIAR
    eSOSShowList, // /SHOW SOS
    eSOSRemove, // SOSDONE
    eGoToChar, // /IRA
    einvisible, // /INVISIBLE
    eGMPanel, // /PANELGM
    eRequestUserList, // LISTUSU
    eWorking, // /TRABAJANDO
    eHiding, // /OCULTANDO
    eJail, // /CARCEL
    eKillNPC, // /RMATA
    eWarnUser, // /ADVERTENCIA
    eEditChar, // /MOD
    eRequestCharInfo, // /INFO
    eRequestCharStats, // /STAT
    eRequestCharGold, // /BAL
    eRequestCharInventory, // /INV
    eRequestCharBank, // /BOV
    eRequestCharSkills, // /SKILLS
    eReviveChar, // /REVIVIR
    eOnlineGM, // /ONLINEGM
    eOnlineMap, // /ONLINEMAP
    eForgive, // /PERDON
    eKick, // /ECHAR
    eExecute, // /EJECUTAR
    eBanChar, // /BAN
    eUnbanChar, // /UNBAN
    eNPCFollow, // /SEGUIR
    eSummonChar, // /SUM
    eSpawnListRequest, // /CC
    eSpawnCreature, // SPA
    eResetNPCInventory, // /RESETINV
    eCleanWorld, // /LIMPIAR
    eServerMessage, // /RMSG
    eNickToIP, // /NICK2IP
    eIPToNick, // /IP2NICK
    eGuildOnlineMembers, // /ONCLAN
    eTeleportCreate, // /CT
    eTeleportDestroy, // /DT
    eRainToggle, // /LLUVIA
    eSetCharDescription, // /SETDESC
    eForceMIDIToMap, // /FORCEMIDIMAP
    eForceWAVEToMap, // /FORCEWAVMAP
    eRoyalArmyMessage, // /REALMSG
    eChaosLegionMessage, // /CAOSMSG
    eTalkAsNPC, // /TALKAS
    eDestroyAllItemsInArea, // /MASSDEST
    eAcceptRoyalCouncilMember, // /ACEPTCONSE
    eAcceptChaosCouncilMember, // /ACEPTCONSECAOS
    eItemsInTheFloor, // /PISO
    eMakeDumb, // /ESTUPIDO
    eMakeDumbNoMore, // /NOESTUPIDO
    eCouncilKick, // /KICKCONSE
    eSetTrigger, // /TRIGGER
    eAskTrigger, // /TRIGGER with no args
    eGuildMemberList, // /MIEMBROSCLAN
    eGuildBan, // /BANCLAN
    eCreateItem, // /CI
    eDestroyItems, // /DEST
    eChaosLegionKick, // /NOCAOS
    eRoyalArmyKick, // /NOREAL
    eForceMIDIAll, // /FORCEMIDI
    eForceWAVEAll, // /FORCEWAV
    eRemovePunishment, // /BORRARPENA
    eTileBlockedToggle, // /BLOQ
    eKillNPCNoRespawn, // /MATA
    eKillAllNearbyNPCs, // /MASSKILL
    eChangeMOTD, // /MOTDCAMBIA
    eSetMOTD, // ZMOTD
    eSystemMessage, // /SMSG
    eCreateNPC, // /ACC
    eCreateNPCWithRespawn, // /RACC
    eNavigateToggle, // /NAVE
    eServerOpenToUsersToggle, // /HABILITAR
    eParticipar, // /PARTICIPAR
    eRemoveCharFromGuild, // /RAJARCLAN
    eAlterName, // /ANAME
    eDoBackUp, // /DOBACKUP
    eShowGuildMessages, // /SHOWCMSG
    eChangeMapInfoPK, // /MODMAPINFO PK
    eChangeMapInfoBackup, // /MODMAPINFO BACKUP
    eChangeMapInfoRestricted, // /MODMAPINFO RESTRINGIR
    eChangeMapInfoNoMagic, // /MODMAPINFO MAGIASINEFECTO
    eChangeMapInfoNoInvi, // /MODMAPINFO INVISINEFECTO
    eChangeMapInfoNoResu, // /MODMAPINFO RESUSINEFECTO
    eChangeMapInfoLand, // /MODMAPINFO TERRENO
    eChangeMapInfoZone, // /MODMAPINFO ZONA
    eChangeMapSetting, // /MODSETTING setting value
    eSaveChars, // /GRABAR
    eCleanSOS, // /BORRAR SOS
    eShowServerForm, // /SHOW INT
    eKickAllChars, // /ECHARTODOSPJS
    eChatColor, // /CHATCOLOR
    eIgnored, // /IGNORADO
    eCheckSlot, // /SLOT
    eSetSpeed, // /SPEED
    eGlobalMessage, // /CONSOLA
    eGlobalOnOff,
    eUseKey,
    eDonateGold, // /DONAR
    ePromedio, // /PROMEDIO
    eGiveItem, // /DAR
    eOfertaInicial,
    eOfertaDeSubasta,
    eQuestionGM,
    eCuentaRegresiva,
    ePossUser,
    eDuel,
    eAcceptDuel,
    eCancelDuel,
    eQuitDuel,
    eNieveToggle,
    eNieblaToggle,
    eTransFerGold,
    eMoveitem,
    eGenio,
    eCasarse,
    eCraftAlquimista,
    eFlagTrabajar,
    eCraftSastre,
    eMensajeUser,
    eTraerBoveda,
    eCompletarAccion,
    eInvitarGrupo,
    eResponderPregunta,
    eRequestGrupo,
    eAbandonarGrupo,
    eHecharDeGrupo,
    eMacroPossent,
    eSubastaInfo,
    eBanCuenta,
    eUnbanCuenta,
    eCerrarCliente,
    eEventoInfo,
    eCrearEvento,
    eBanTemporal,
    eCancelarExit,
    eCrearTorneo,
    eComenzarTorneo,
    eCancelarTorneo,
    eBusquedaTesoro,
    eCompletarViaje,
    eBovedaMoveItem,
    eQuieroFundarClan,
    ellamadadeclan,
    eMarcaDeClanPack,
    eMarcaDeGMPack,
    eQuest,
    eQuestAccept,
    eQuestListRequest,
    eQuestDetailsRequest,
    eQuestAbandon,
    eSeguroClan,
    ehome, // /HOGAR
    eConsulta, // /CONSULTA
    eGetMapInfo, // /MAPINFO
    eFinEvento,
    eSeguroResu,
    eLegionarySecure,
    eCuentaExtractItem,
    eCuentaDeposit,
    eCreateEvent,
    eCommerceSendChatMessage,
    eLogMacroClickHechizo,
    eAddItemCrafting,
    eRemoveItemCrafting,
    eAddCatalyst,
    eRemoveCatalyst,
    eCraftItem,
    eCloseCrafting,
    eMoveCraftItem,
    ePetLeaveAll,
    eResetChar, // /RESET NICK
    eResetearPersonaje,
    eDeleteItem,
    eFinalizarPescaEspecial,
    eRomperCania,
    eUseItemU,
    eRepeatMacro,
    eBuyShopItem,
    eStartEvent, // /EVENTO CAPTURA/LOBBY
    eCancelarEvento, // /CANCELAREVENTO
    eNotifyInventarioHechizos,
    ePublicarPersonajeMAO,
    eEventoFaccionario,
    eRequestDebug, // /RequestDebug consulta info debug al server, para gms
    eLobbyCommand,
    eFeatureToggle,
    eActionOnGroupFrame,
    eSetHotkeySlot,
    eUseHKeySlot,
    eAntiCheatMessage,
    eRequestLobbyList,
    eCreateAccount,
    eLoginAccount,
    eDeleteCharacter,
    eChangeSkinSlot,
    eStartAutomatedAction,
    ePetFollowAll,
    eAntiMacroMessage,
    eModifyCastleWhiteList,
    eHooClientCapabilities,
    eRequestRemort,
    eHooTargetedSpellCast,
    eHooHouseDoorAction,
    eMaxPacket,
}

