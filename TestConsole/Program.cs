﻿using System;
using VkTools;
using VkTools.Serializers;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializer = new NewsFeedDeserializer();
            var result = serializer.Deserialize("{\"response\":{\"items\":[{\"type\":\"post\",\"source_id\":-92876084,\"date\":1550418900,\"post_id\":191033,\"post_type\":\"post\",\"text\":\"Идёт пьяный мужик по улице. Видит какую-то красную хуйню.\",\"signer_id\":11523171,\"marked_as_ads\":0,\"attachments\":[{\"type\":\"photo\",\"photo\":{\"id\":456262203,\"album_id\":-7,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-31.u...cc4/3dkYaFowdc8.jpg\",\"width\":54,\"height\":130},{\"type\":\"o\",\"url\":\"https://sun9-32.u...cc6/ORn0M4OTaxk.jpg\",\"width\":130,\"height\":313},{\"type\":\"p\",\"url\":\"https://sun9-30.u...cc7/qKeNnW2rXos.jpg\",\"width\":200,\"height\":481},{\"type\":\"q\",\"url\":\"https://sun9-13.u...cc8/IDniRlmVQRg.jpg\",\"width\":251,\"height\":604},{\"type\":\"r\",\"url\":\"https://sun9-3.us...cc9/B3EPagO3cuY.jpg\",\"width\":251,\"height\":604},{\"type\":\"s\",\"url\":\"https://sun9-4.us...cc3/DCnnR55qp3k.jpg\",\"width\":31,\"height\":75},{\"type\":\"x\",\"url\":\"https://sun9-11.u...cc5/OUv2ciNkJlU.jpg\",\"width\":251,\"height\":604}],\"text\":\"\",\"date\":1550040470,\"access_key\":\"e26fb730771ec8fc13\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262204,\"album_id\":-7,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-27.u...ccb/8vM_l_GrUyk.jpg\",\"width\":68,\"height\":130},{\"type\":\"o\",\"url\":\"https://sun9-27.u...ccd/0jvfVb7WNDU.jpg\",\"width\":130,\"height\":250},{\"type\":\"p\",\"url\":\"https://sun9-34.u...cce/OrAQ9vgh-hw.jpg\",\"width\":200,\"height\":385},{\"type\":\"q\",\"url\":\"https://sun9-21.u...ccf/08pOUhe_qrY.jpg\",\"width\":314,\"height\":604},{\"type\":\"r\",\"url\":\"https://sun9-31.u...cd0/K5VtQXt49uU.jpg\",\"width\":314,\"height\":604},{\"type\":\"s\",\"url\":\"https://sun9-14.u...cca/Uax-v3sYa7Q.jpg\",\"width\":39,\"height\":75},{\"type\":\"x\",\"url\":\"https://sun9-32.u...ccc/iRXyNHIsKSo.jpg\",\"width\":314,\"height\":604}],\"text\":\"\",\"date\":1550040470,\"access_key\":\"a3ac5d4d6367c09287\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262205,\"album_id\":-7,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-10.u...cd2/yX6DkJ80ha8.jpg\",\"width\":62,\"height\":130},{\"type\":\"o\",\"url\":\"https://sun9-13.u...cd4/1Fpa_0rOmfc.jpg\",\"width\":130,\"height\":271},{\"type\":\"p\",\"url\":\"https://sun9-9.us...cd5/YkmJVONcvHI.jpg\",\"width\":200,\"height\":417},{\"type\":\"q\",\"url\":\"https://sun9-8.us...cd6/cDTmSu7QAxY.jpg\",\"width\":290,\"height\":604},{\"type\":\"r\",\"url\":\"https://sun9-1.us...cd7/EAFQyfWcjdo.jpg\",\"width\":290,\"height\":604},{\"type\":\"s\",\"url\":\"https://sun9-27.u...cd1/3vQ_AmUl6H0.jpg\",\"width\":36,\"height\":75},{\"type\":\"x\",\"url\":\"https://sun9-10.u...cd3/B7NVS1TQTH4.jpg\",\"width\":290,\"height\":604}],\"text\":\"\",\"date\":1550040470,\"access_key\":\"25f42669ed85dabe68\"}}],\"post_source\":{\"type\":\"vk\"},\"comments\":{\"count\":0,\"can_post\":0,\"groups_can_post\":true},\"likes\":{\"count\":263,\"user_likes\":0,\"can_like\":1,\"can_publish\":1},\"reposts\":{\"count\":3,\"user_reposted\":0},\"views\":{\"count\":5948},\"is_favorite\":false},{\"type\":\"post\",\"source_id\":-92876084,\"date\":1550415300,\"post_id\":191020,\"post_type\":\"post\",\"text\":\"[club108321657|внутри тебя ждет куча татуировок, загляни]\",\"marked_as_ads\":1,\"attachments\":[{\"type\":\"photo\",\"photo\":{\"id\":456262420,\"album_id\":-51,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-28.u...694/m9YlQCAW35o.jpg\",\"width\":130,\"height\":129},{\"type\":\"o\",\"url\":\"https://sun9-13.u...697/g5fyrIt0YBc.jpg\",\"width\":130,\"height\":129},{\"type\":\"p\",\"url\":\"https://sun9-26.u...698/0p5NUq6Wl2w.jpg\",\"width\":200,\"height\":198},{\"type\":\"q\",\"url\":\"https://sun9-4.us...699/RZVpUUzMqrA.jpg\",\"width\":320,\"height\":317},{\"type\":\"r\",\"url\":\"https://sun9-2.us...69a/hp3dsqt_rQ4.jpg\",\"width\":510,\"height\":506},{\"type\":\"s\",\"url\":\"https://sun9-30.u...693/i6BLVW8GHrQ.jpg\",\"width\":75,\"height\":74},{\"type\":\"x\",\"url\":\"https://sun9-30.u...695/DoaX9hEurv4.jpg\",\"width\":604,\"height\":599},{\"type\":\"y\",\"url\":\"https://sun9-7.us...696/2xX9Rb0IdAc.jpg\",\"width\":750,\"height\":744}],\"text\":\"\",\"date\":1550341626,\"access_key\":\"661df4e7c2b1b9aa0c\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262421,\"album_id\":-51,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-21.u...1b3/XOzY4GWmamc.jpg\",\"width\":130,\"height\":130},{\"type\":\"o\",\"url\":\"https://sun9-9.us...1b7/PX-HkXnGEJA.jpg\",\"width\":130,\"height\":130},{\"type\":\"p\",\"url\":\"https://sun9-21.u...1b8/w7-vDhNn9IY.jpg\",\"width\":200,\"height\":200},{\"type\":\"q\",\"url\":\"https://sun9-28.u...1b9/vO1TpSFxCUU.jpg\",\"width\":320,\"height\":320},{\"type\":\"r\",\"url\":\"https://sun9-21.u...1ba/joGwIIDEc4s.jpg\",\"width\":510,\"height\":510},{\"type\":\"s\",\"url\":\"https://sun9-30.u...1b2/jMi2xezuujw.jpg\",\"width\":75,\"height\":75},{\"type\":\"x\",\"url\":\"https://sun9-30.u...1b4/vQTDvoPiTxY.jpg\",\"width\":604,\"height\":604},{\"type\":\"y\",\"url\":\"https://sun9-30.u...1b5/WKkoAu-mQK0.jpg\",\"width\":807,\"height\":807},{\"type\":\"z\",\"url\":\"https://sun9-31.u...1b6/F8frkht-QYc.jpg\",\"width\":1080,\"height\":1080}],\"text\":\"\",\"date\":1550341626,\"access_key\":\"974d70fd1ace9eff7b\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262422,\"album_id\":-51,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-5.us...0a6/66WnhESL7Cw.jpg\",\"width\":130,\"height\":130},{\"type\":\"o\",\"url\":\"https://sun9-9.us...0a9/koTtkTsAd_k.jpg\",\"width\":130,\"height\":130},{\"type\":\"p\",\"url\":\"https://sun9-13.u...0aa/XFpd4z136x4.jpg\",\"width\":200,\"height\":200},{\"type\":\"q\",\"url\":\"https://sun9-27.u...0ab/6Mm1CA_IgsU.jpg\",\"width\":320,\"height\":320},{\"type\":\"r\",\"url\":\"https://sun9-26.u...0ac/ZKB34KHwfCo.jpg\",\"width\":510,\"height\":510},{\"type\":\"s\",\"url\":\"https://sun9-5.us...0a5/2v_68lhBBDQ.jpg\",\"width\":75,\"height\":75},{\"type\":\"x\",\"url\":\"https://sun9-27.u...0a7/WJQxGIUVhFA.jpg\",\"width\":604,\"height\":604},{\"type\":\"y\",\"url\":\"https://sun9-2.us...0a8/Bfwh-DZzJmc.jpg\",\"width\":640,\"height\":640}],\"text\":\"\",\"date\":1550341626,\"access_key\":\"d080b88fe111e91d4d\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262423,\"album_id\":-51,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-11.u...ddb/LR4q-zMUc2o.jpg\",\"width\":130,\"height\":130},{\"type\":\"o\",\"url\":\"https://sun9-7.us...dde/aSRESEc4-co.jpg\",\"width\":130,\"height\":130},{\"type\":\"p\",\"url\":\"https://sun9-22.u...ddf/y-t46RNVhwo.jpg\",\"width\":200,\"height\":200},{\"type\":\"q\",\"url\":\"https://sun9-27.u...de0/zoxGBD9A-tM.jpg\",\"width\":320,\"height\":320},{\"type\":\"r\",\"url\":\"https://sun9-4.us...de1/POvuUYu5f74.jpg\",\"width\":510,\"height\":510},{\"type\":\"s\",\"url\":\"https://sun9-11.u...dda/zcTEU-g_iAY.jpg\",\"width\":75,\"height\":75},{\"type\":\"x\",\"url\":\"https://sun9-3.us...ddc/VXLyjvqVWAc.jpg\",\"width\":604,\"height\":604},{\"type\":\"y\",\"url\":\"https://sun9-5.us...ddd/j6liB9Fcb_s.jpg\",\"width\":689,\"height\":689}],\"text\":\"\",\"date\":1550341626,\"access_key\":\"9e0176e00393a036f7\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262424,\"album_id\":-51,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-27.u...d2a/rYMuMWeqWps.jpg\",\"width\":130,\"height\":130},{\"type\":\"o\",\"url\":\"https://sun9-2.us...d2e/hsLQ4VIrczE.jpg\",\"width\":130,\"height\":130},{\"type\":\"p\",\"url\":\"https://sun9-33.u...d2f/qblPSS8DgfA.jpg\",\"width\":200,\"height\":200},{\"type\":\"q\",\"url\":\"https://sun9-25.u...d30/Mf2ffccaUHw.jpg\",\"width\":320,\"height\":320},{\"type\":\"r\",\"url\":\"https://sun9-29.u...d31/gveqQKtcWm8.jpg\",\"width\":510,\"height\":510},{\"type\":\"s\",\"url\":\"https://sun9-29.u...d29/Yi2txZJMrXg.jpg\",\"width\":75,\"height\":75},{\"type\":\"x\",\"url\":\"https://sun9-6.us...d2b/IETlShZI0KY.jpg\",\"width\":604,\"height\":604},{\"type\":\"y\",\"url\":\"https://sun9-29.u...d2c/wnUsTB6LlDc.jpg\",\"width\":807,\"height\":807},{\"type\":\"z\",\"url\":\"https://sun9-2.us...d2d/TedwPZkdSkk.jpg\",\"width\":1080,\"height\":1080}],\"text\":\"\",\"date\":1550341626,\"access_key\":\"9c26bb15466ddfbdc0\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262425,\"album_id\":-51,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-30.u...5ca/SR4D2uCbK58.jpg\",\"width\":130,\"height\":130},{\"type\":\"o\",\"url\":\"https://sun9-7.us...5ce/Xph3PIMRWJE.jpg\",\"width\":130,\"height\":130},{\"type\":\"p\",\"url\":\"https://sun9-33.u...5cf/YuKudVdXaZc.jpg\",\"width\":200,\"height\":200},{\"type\":\"q\",\"url\":\"https://sun9-23.u...5d0/lPsh18RsN6g.jpg\",\"width\":320,\"height\":320},{\"type\":\"r\",\"url\":\"https://sun9-30.u...5d1/v1UG8tiCPsM.jpg\",\"width\":510,\"height\":510},{\"type\":\"s\",\"url\":\"https://sun9-27.u...5c9/M4uA7RKrbSk.jpg\",\"width\":75,\"height\":75},{\"type\":\"x\",\"url\":\"https://sun9-23.u...5cb/43nW0c5uE_4.jpg\",\"width\":604,\"height\":604},{\"type\":\"y\",\"url\":\"https://sun9-26.u...5cc/L5jR5immvho.jpg\",\"width\":807,\"height\":807},{\"type\":\"z\",\"url\":\"https://sun9-8.us...5cd/hnemVJqGse0.jpg\",\"width\":1080,\"height\":1080}],\"text\":\"\",\"date\":1550341626,\"access_key\":\"d9d7b04120b445ad89\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262426,\"album_id\":-51,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-7.us...caa/l7d44bzYdsQ.jpg\",\"width\":130,\"height\":130},{\"type\":\"o\",\"url\":\"https://sun9-27.u...cae/7HmO4T8Px_E.jpg\",\"width\":130,\"height\":130},{\"type\":\"p\",\"url\":\"https://sun9-29.u...caf/learmdEgjH8.jpg\",\"width\":200,\"height\":200},{\"type\":\"q\",\"url\":\"https://sun9-24.u...cb0/8AUCWr1vZEQ.jpg\",\"width\":320,\"height\":320},{\"type\":\"r\",\"url\":\"https://sun9-34.u...cb1/dprFOPynHI0.jpg\",\"width\":510,\"height\":510},{\"type\":\"s\",\"url\":\"https://sun9-10.u...ca9/8Q69qQl6q5Y.jpg\",\"width\":75,\"height\":75},{\"type\":\"x\",\"url\":\"https://sun9-1.us...cab/zVFikwUCQ6w.jpg\",\"width\":604,\"height\":604},{\"type\":\"y\",\"url\":\"https://sun9-34.u...cac/84C4SSFygr0.jpg\",\"width\":807,\"height\":807},{\"type\":\"z\",\"url\":\"https://sun9-1.us...cad/BfDmwMnXk8c.jpg\",\"width\":960,\"height\":960}],\"text\":\"\",\"date\":1550341626,\"access_key\":\"ef9c060eb346e5ff55\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262427,\"album_id\":-51,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-12.u...20e/Z9hgtWihjnw.jpg\",\"width\":130,\"height\":87},{\"type\":\"o\",\"url\":\"https://sun9-1.us...212/WoJwARb1PcQ.jpg\",\"width\":130,\"height\":87},{\"type\":\"p\",\"url\":\"https://sun9-33.u...213/Ougw2jGOMQs.jpg\",\"width\":200,\"height\":133},{\"type\":\"q\",\"url\":\"https://sun9-12.u...214/k8lQV_2cV5g.jpg\",\"width\":320,\"height\":213},{\"type\":\"r\",\"url\":\"https://sun9-8.us...215/alcQhFInQiA.jpg\",\"width\":510,\"height\":340},{\"type\":\"s\",\"url\":\"https://sun9-30.u...20d/i0ebyHSX99E.jpg\",\"width\":75,\"height\":50},{\"type\":\"x\",\"url\":\"https://sun9-23.u...20f/FY8Y64gm9Wk.jpg\",\"width\":604,\"height\":402},{\"type\":\"y\",\"url\":\"https://sun9-29.u...210/n7a-xjbIAKw.jpg\",\"width\":807,\"height\":537},{\"type\":\"z\",\"url\":\"https://sun9-13.u...211/d62MlrqupEc.jpg\",\"width\":1080,\"height\":718}],\"text\":\"\",\"date\":1550341626,\"access_key\":\"f0eb0330fcf3f30c6b\"}},{\"type\":\"photo\",\"photo\":{\"id\":456262428,\"album_id\":-51,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-23.u...2ee/G15G2v6BKXs.jpg\",\"width\":130,\"height\":129},{\"type\":\"o\",\"url\":\"https://sun9-29.u...2f2/s-7Uv6Fgno4.jpg\",\"width\":130,\"height\":129},{\"type\":\"p\",\"url\":\"https://sun9-3.us...2f3/QAoqc69HtiQ.jpg\",\"width\":200,\"height\":199},{\"type\":\"q\",\"url\":\"https://sun9-21.u...2f4/wNi0Hemdvmo.jpg\",\"width\":320,\"height\":318},{\"type\":\"r\",\"url\":\"https://sun9-21.u...2f5/Ow2YMKxpHSA.jpg\",\"width\":510,\"height\":506},{\"type\":\"s\",\"url\":\"https://sun9-25.u...2ed/Jo6VyRLiWGE.jpg\",\"width\":75,\"height\":74},{\"type\":\"x\",\"url\":\"https://sun9-14.u...2ef/GSQ_zAA6YpM.jpg\",\"width\":604,\"height\":600},{\"type\":\"y\",\"url\":\"https://sun9-23.u...2f0/dEuhXEEhxLM.jpg\",\"width\":807,\"height\":801},{\"type\":\"z\",\"url\":\"https://sun9-4.us...2f1/blDsgLVtTpA.jpg\",\"width\":960,\"height\":953}],\"text\":\"\",\"date\":1550341626,\"access_key\":\"9513d690117b7469c8\"}}],\"post_source\":{\"type\":\"vk\"},\"comments\":{\"count\":0,\"can_post\":0,\"groups_can_post\":true},\"likes\":{\"count\":27,\"user_likes\":0,\"can_like\":1,\"can_publish\":1},\"reposts\":{\"count\":0,\"user_reposted\":0},\"views\":{\"count\":15474},\"is_favorite\":false},{\"type\":\"post\",\"source_id\":-92876084,\"date\":1550413680,\"post_id\":191015,\"post_type\":\"post\",\"text\":\"Непонятное, но забавное и интригующее начало\\n.\\n.\\n.\\n.\\n.\\n. \\n. \\n. \\n. \\n. \\n. \\n. \\n. \\n. \\n.\\n. \\n. \\n. \\n. \\n. \\n. \\n. \\n. \\n. \\n.\\n.\\n.\\n.\\n.\\n.\\n.\\n.\\n.\\n.\\nхуйня хуйнёй\",\"signer_id\":255543352,\"marked_as_ads\":0,\"post_source\":{\"type\":\"vk\"},\"comments\":{\"count\":0,\"can_post\":0,\"groups_can_post\":true},\"likes\":{\"count\":754,\"user_likes\":0,\"can_like\":1,\"can_publish\":1},\"reposts\":{\"count\":11,\"user_reposted\":0},\"views\":{\"count\":23413},\"is_favorite\":false},{\"type\":\"post\",\"source_id\":-92876084,\"date\":1550411700,\"post_id\":191008,\"post_type\":\"post\",\"text\":\"\",\"signer_id\":139535325,\"marked_as_ads\":0,\"attachments\":[{\"type\":\"photo\",\"photo\":{\"id\":456262202,\"album_id\":-7,\"owner_id\":-92876084,\"user_id\":100,\"sizes\":[{\"type\":\"m\",\"url\":\"https://sun9-13.u...ab4/-LtsgCrXCtI.jpg\",\"width\":130,\"height\":53},{\"type\":\"o\",\"url\":\"https://sun9-9.us...ab9/3JTQ-1k9Grs.jpg\",\"width\":130,\"height\":87},{\"type\":\"p\",\"url\":\"https://sun9-33.u...aba/CwEKhfMIEP4.jpg\",\"width\":200,\"height\":133},{\"type\":\"q\",\"url\":\"https://sun9-29.u...abb/e1NZ9iIGnbQ.jpg\",\"width\":320,\"height\":213},{\"type\":\"r\",\"url\":\"https://sun9-30.u...abc/asIFPcn8uUY.jpg\",\"width\":510,\"height\":340},{\"type\":\"s\",\"url\":\"https://sun9-11.u...ab3/buxRQKUZA1w.jpg\",\"width\":75,\"height\":31},{\"type\":\"w\",\"url\":\"https://sun9-2.us...ab8/z1ezP1tqqFs.jpg\",\"width\":1563,\"height\":640},{\"type\":\"x\",\"url\":\"https://sun9-6.us...ab5/SqtvnPBkeLY.jpg\",\"width\":604,\"height\":247},{\"type\":\"y\",\"url\":\"https://sun9-29.u...ab6/nH4yBJ0dUGE.jpg\",\"width\":807,\"height\":330},{\"type\":\"z\",\"url\":\"https://sun9-24.u...ab7/nk6hQoIWpBc.jpg\",\"width\":1280,\"height\":524}],\"text\":\"\",\"date\":1550038824,\"lat\":54.344065,\"long\":48.539378,\"post_id\":189734,\"access_key\":\"23e6310f321b9b976b\"}}],\"post_source\":{\"type\":\"vk\"},\"comments\":{\"count\":0,\"can_post\":0,\"groups_can_post\":true},\"likes\":{\"count\":782,\"user_likes\":0,\"can_like\":1,\"can_publish\":1},\"reposts\":{\"count\":10,\"user_reposted\":0},\"views\":{\"count\":29686},\"is_favorite\":false},{\"type\":\"post\",\"source_id\":-92876084,\"date\":1550406900,\"post_id\":190994,\"post_type\":\"post\",\"text\":\"Идут по дороге белоснежка, дюймовочка и Список Шиндлера Белоснежка говорит:\",\"marked_as_ads\":0,\"post_source\":{\"type\":\"vk\"},\"comments\":{\"count\":0,\"can_post\":0,\"groups_can_post\":true},\"likes\":{\"count\":437,\"user_likes\":0,\"can_like\":1,\"can_publish\":1},\"reposts\":{\"count\":9,\"user_reposted\":0},\"views\":{\"count\":29696},\"is_favorite\":false}],\"profiles\":[{\"id\":11523171,\"first_name\":\"Sergey\",\"last_name\":\"Vinograd\",\"is_closed\":false,\"can_access_closed\":true,\"sex\":2,\"screen_name\":\"avu_khur\",\"photo_50\":\"https://sun9-28.u...E-5v2aiBY.jpg?ava=1\",\"photo_100\":\"https://sun9-3.us...hANacDfJs.jpg?ava=1\",\"online\":1,\"online_app\":\"2685278\",\"online_mobile\":1},{\"id\":139535325,\"first_name\":\"Alexander\",\"last_name\":\"Butenko\",\"is_closed\":false,\"can_access_closed\":true,\"sex\":2,\"screen_name\":\"butenkoa\",\"photo_50\":\"https://sun9-14.u...-A8qckBwA.jpg?ava=1\",\"photo_100\":\"https://sun9-10.u...AlDO2bb4U.jpg?ava=1\",\"online\":1},{\"id\":255543352,\"first_name\":\"Anton\",\"last_name\":\"Gulyashinov\",\"is_closed\":false,\"can_access_closed\":true,\"sex\":2,\"screen_name\":\"antonghul\",\"photo_50\":\"https://sun9-10.u...ozBTrJloI.jpg?ava=1\",\"photo_100\":\"https://sun9-9.us...T0gcdBuEs.jpg?ava=1\",\"online\":0}],\"groups\":[{\"id\":92876084,\"name\":\"Мои любимые юморески\",\"screen_name\":\"jumoreski\",\"is_closed\":0,\"type\":\"page\",\"is_admin\":0,\"is_member\":1,\"is_advertiser\":0,\"photo_50\":\"https://sun9-14.u...wq2QqrRVI.jpg?ava=1\",\"photo_100\":\"https://sun9-11.u...DdMisdTgQ.jpg?ava=1\",\"photo_200\":\"https://sun9-5.us...x8fqzEwQo.jpg?ava=1\"},{\"id\":108321657,\"name\":\"тоту\",\"screen_name\":\"totytoty\",\"is_closed\":0,\"type\":\"page\",\"is_admin\":0,\"is_member\":0,\"is_advertiser\":0,\"photo_50\":\"https://sun9-9.us...32LNk8lYI.jpg?ava=1\",\"photo_100\":\"https://sun9-6.us...3h2yXEABs.jpg?ava=1\",\"photo_200\":\"https://sun9-32.u...DH9IvA_JI.jpg?ava=1\"},{\"id\":171780078,\"name\":\"Блог Дмитрия Соколова | Как заработать\",\"screen_name\":\"blogsokolov\",\"is_closed\":0,\"type\":\"page\",\"is_admin\":0,\"is_member\":0,\"is_advertiser\":0,\"photo_50\":\"https://sun9-27.u...N_ByjAa34.jpg?ava=1\",\"photo_100\":\"https://sun9-26.u...s98uYcbIA.jpg?ava=1\",\"photo_200\":\"https://sun9-32.u...0n_9FEakc.jpg?ava=1\"}],\"next_from\":\"5/5_-92876084_190994:1768048338\"}}");

            Console.ReadKey();
        }
    }
}
