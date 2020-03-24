﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/* ==============================================================================
  * 类名称：Constant
  * 类描述：
  * 创建人：later
  * 创建时间：2020/3/24 9:52:06
  * 修改人：
  * 修改时间：
  * 修改备注：
  * @version 1.0
  * ==============================================================================*/
namespace ChatClient
{

    class Constant
    {
        public static String testPicStr = "FFD8FFE000104A46494600010101000000000000FFDB0043000C08090B09080C0B0A0B0E0D0C0E121E1412111112251A1C161E2C262E2D2B262A293036453B30334134292A3C523D41474A4D4E4D2F3A555B544B5A454C4D4AFFDB0043010D0E0E121012231414234A322A324A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4AFFC4001F0000010501010101010100000000000000000102030405060708090A0BFFC400B5100002010303020403050504040000017D01020300041105122131410613516107227114328191A1082342B1C11552D1F02433627282090A161718191A25262728292A3435363738393A434445464748494A535455565758595A636465666768696A737475767778797A838485868788898A92939495969798999AA2A3A4A5A6A7A8A9AAB2B3B4B5B6B7B8B9BAC2C3C4C5C6C7C8C9CAD2D3D4D5D6D7D8D9DAE1E2E3E4E5E6E7E8E9EAF1F2F3F4F5F6F7F8F9FAFFC4001F0100030101010101010101010000000000000102030405060708090A0BFFC400B51100020102040403040705040400010277000102031104052131061241510761711322328108144291A1B1C109233352F0156272D10A162434E125F11718191A262728292A35363738393A434445464748494A535455565758595A636465666768696A737475767778797A82838485868788898A92939495969798999AA2A3A4A5A6A7A8A9AAB2B3B4B5B6B7B8B9BAC2C3C4C5C6C7C8C9CAD2D3D4D5D6D7D8D9DAE2E3E4E5E6E7E8E9EAF2F3F4F5F6F7F8F9FAFFC000110800F0014003012100021101031101FFDA000C03010002110311003F00E068A8330A5A420A280168A0414B40051400B45001450014500145001450014530128A4014C6A6521B4559A05140051400514005140051400EA2A0CC5A290829681051400B45002D140051400B45001450014500145001450025140094C6EB4D1486D1566814500145001450014500145003A96B3320A280168A0414B40052D0014B4005140052D00145001450014500251400525001511EB548A8894551A05140051400514005140051400FA2B3320A5A04145002D1400B45002D1400B450014500145001450014500145002514009511EB548A8894551A05140051400514005140051400FA2B3320A5A0414B40052D0014B40052D00145002D1400514005140051400525001494005427AD522A2251546814500145001450014500145003E8ACCC8296810B450014B400B45002D1400B450014B40051400514005140094500251400951375AA45446D1546814500145001450014500145003E8ACCC85A2810B45002D1400B4B40051400B45002D140051400514005140052500149400542FF007A9A2A2368AB340A2800A2800A2800A2800A2801F4566642D14085A5A00296800A5A402D14C05A2800A5A06145020A281851400945021292800A89FAD3452194559A05140051400514005140051400FA2B332168A042D1400B4B400B4520169698052D030A2800A2800A2800A2800A4A04251400951C94D1488E8AB340A2800A2800A2800A2800A2801F4B5998852D0014B40052D002D2D0014B400B45002D140C28A0028A0028A004A281094940094C7E94C688A8AB350A2800A2800A2800A2800A28024A2B3310A5A0029680168A005A280169680168A062D1400B45001450014940086A12D400DDC68DC698EC2669298C4A2A8B0A2800A2800A2800A2800A2801F4B506214B480296801696800A280169680169681852D0014500145001450034D56340094532828A63128AA2828A0028A0028A0028A0028A00928ACCC45A2800A5A005A5A0029680168A062D2D002D140052D00145002514008C38AAD4009494CA0A298C4A2A8A0A2800A2800A2800A2800A2801F4B5998852D002D1400B4B40052D002D2D030A5A005A2800A5A00292800A5A0069E9558D002514C62514C62515458514005140051400514005140125159988B45002D1400B4B40052D002D2D0014B40C29680168A0028A0028A0043D2AA9A004A298C4A29942514C6145318514005140051400514012515998852D002D1400B4B40052D002D2D030A5A005A280168A0028A0028A006B74AAC680128A631292994145318514C6145001450014500145004945419052D21052D002D1400B4B400B45002D2D030A5A00296800A280128A0043D2AB1A004A298C4A4A650514C614531851400514005140051400FA5ACCC829681052D0014B4805A5A602D1400B4B40C296800A2800A2800A28010F4AAC68012929942515450514C614500145001450014500145003E8ACCC85A2810B4B40052D002D2D0014B400B450014B40C296800A2800A2980D3D2AB9A402525328292A8A0A298C28A0028A0028A0028A0028A007D150642D14842D1400B4B40052D002D1400B4B40052D030A2800A2800A298087A55734804A4A6509455141453185140051400514005140051400FA2B332168A0414B400B4B40052D0014B400B45002D140C28A0029680128A6021E9501A40368A6509455141453185140051400514005140051400FA2A0C829690828A002945003A8A005A5A0028A005A2800A2800A5A004CD14C00F4AAE681A128A0A128AA2828A630A2800A2800A2800A2800A2801D4B506414520168A0414B400B45002D1400B45001471400B45002D14005250007A540698D09450509455141453185140051400514005140051400EA2A0CC5A290829681052D002D140052D002D140052D001450014B400945002ED2693ECCC6818BF65F7A77D947AD002FD9569AD6E83BD03B8C68907F1547B39EB4EE1CC3BECED4C3138ED4EE5730DA4AA2828A0028A0028A00752D41985148414B408296800A5A00296800A280168A0028A005A5009A00788BD69E1545003C114FA0031462800A4A006941E94C30AD00380C51400D233519854F6A0644D6FE86A328C3B555CBB8CA2A8A0A2801D454198B4521051400B4502168A00296800A5A0028A005A788CD003C28A760FD2818A23FF6A9E235A043B60A4D9EF400D90BFF0008A8FF0079DE80232FCF7A904BC5301CB203D69F4802928012928012928018D1A9A85A0F4A6526454956683A8A8330A5A420A2800A5A0414B40051400B45003C2134FD8075A0037E3A0A6195FD28013CC3DA9D991BAD301E3E5EAD52AB8A007EFA81AE0E78A403848D4A27E714C09383DA9AD1AB520207B6FEE9A6F94EBDE81879E5474A3ED5ED4C2C384F9ED5206CD210514C04A43401566FBD51D52345B0EA2A490A5A0028A420A280168A62169429A404823A9020A063F14B8A0050A28C50002214E68F22A1C886C80DBF3D697CA35650796556A0F9BF8A980EA7C7B4D004E290E7B5000A4F7A5A40359430E6A136C3B5000B06D352D30128A4021A89DA9815DA9B546A875152485140829690828A00514F09408902014EE05002EEA50D40C76EA5DC2810EC8A75003969841ACEDA936118EDA6F9C2B4288CCF479CBDE980650D3D557B5004945200A298094520129280129298889DAA027340C61A4AA3542D2D492145020A5A00704A784A448FE05216A604659A9FD56801CBF769A8D8A00977AD3BE5A402ED14983400A3753BCCA005DC0D1B14D302378462AA989A80139EF4090AF4A605A59C6DE695260F480968A04252500149400544C6802BB1A6D22869A4AB4688FFD9";

        public static String SERVER_ADDRESS = "192.168.0.8";//"192.168.1.102";
        public static String SEVER_PORT = "8089";
    }
}
