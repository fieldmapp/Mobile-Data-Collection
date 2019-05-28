BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "QuestionsStadium" (
	"QuestionId"	INTEGER NOT NULL,
	"Difficulty"	INTEGER,
	"Skill"	TEXT,
	"Text"	TEXT,
	"SourcePictureSmall"	TEXT,
	"SourcePictureBig"	TEXT,
	"CorrectAnswer1"	TEXT,
	"CorrectAnswer2"	TEXT,
	PRIMARY KEY("QuestionId")
);
CREATE TABLE IF NOT EXISTS "QuestionsDoubleSlider" (
	"QuestionId"	INTEGER NOT NULL,
	"Difficulty"	INTEGER,
	"Skill"	TEXT,
	"Text"	TEXT,
	"SourcePictureSmall"	TEXT,
	"SourcePictureBig"	TEXT,
	"CorrectPercentageA"	INTEGER,
	"CorrectPercentageB"	INTEGER,
	PRIMARY KEY("QuestionId")
);
CREATE TABLE IF NOT EXISTS "QuestionsIntrospection" (
	"QuestionId"	INTEGER NOT NULL,
	"Skill"	TEXT,
	"Text"	TEXT,
	PRIMARY KEY("QuestionId")
);
CREATE TABLE IF NOT EXISTS "QuestionsImageChecker" (
	"QuestionId"	INTEGER NOT NULL,
	"Difficulty"	INTEGER,
	"Skill"	TEXT,
	"Text"	TEXT,
	"SourcePictureASmall"	TEXT,
	"SourcePictureABig"	TEXT,
	"SourcePictureBSmall"	TEXT,
	"SourcePictureBBig"	TEXT,
	"SourcePictureCSmall"	TEXT,
	"SourcePictureCBig"	TEXT,
	"SourcePictureDSmall"	TEXT,
	"SourcePictureDBig"	TEXT,
	"PictureACorrect"	INTEGER,
	"PictureBCorrect"	INTEGER,
	"PictureCCorrect"	INTEGER,
	"PictureDCorrect"	INTEGER,
	PRIMARY KEY("QuestionId")
);
CREATE TABLE IF NOT EXISTS "AllQuestion" (
	"QuestionId"	INTEGER NOT NULL,
	"QuestionType"	TEXT,
	PRIMARY KEY("QuestionId")
);
INSERT INTO "QuestionsImageChecker" VALUES (1,1,'Crop','Wo sehen Sie die Feldfruchtsorte Weizen abgebildet?','Q1_G1_F1_B1_klein.png','Q1_G1_F1_B1.png','Q1_G1_F1_B2_klein.png','Q1_G1_F1_B2.png','Q1_G1_F1_B3_klein.png','Q1_G1_F1_B3.png','Q1_G1_F1_B4_klein.png','Q1_G1_F1_B4.png',0,1,0,0);
INSERT INTO "QuestionsImageChecker" VALUES (2,1,'Crop','Wo sehen Sie die Feldfruchtsorte Raps abgebildet?','Q1_G1_F2_B1_klein.png','Q1_G1_F2_B1.png','Q1_G1_F2_B2_klein.png','Q1_G1_F2_B2.png','Q1_G1_F2_B3_klein.png','Q1_G1_F2_B3.png','Q1_G1_F2_B4_klein.png','Q1_G1_F2_B4.png',0,1,1,0);
INSERT INTO "QuestionsImageChecker" VALUES (3,2,'Crop','Wo sehen Sie die Feldfruchtsorte Raps abgebildet?','Q1_G2_F1_B1_klein.png','Q1_G2_F1_B1.png','Q1_G2_F1_B2_klein.png','Q1_G2_F1_B2.png','Q1_G2_F1_B3_klein.png','Q1_G2_F1_B3.png','Q1_G2_F1_B4_klein.png','Q1_G2_F1_B4.png',1,1,1,0);
INSERT INTO "QuestionsImageChecker" VALUES (4,2,'Crop','Wo sehen Sie die Feldfruchtsorte Weizen abgebildet?','Q1_G2_F2_B1_klein.png','Q1_G2_F2_B1.png','Q1_G2_F2_B2_klein.png','Q1_G2_F2_B2.png','Q1_G2_F2_B3_klein.png','Q1_G2_F2_B3.png','Q1_G2_F2_B4_klein.png','Q1_G2_F2_B4.png',0,0,1,0);
INSERT INTO "QuestionsImageChecker" VALUES (5,3,'Crop','Wo sehen Sie die Feldfruchtsorte Kartoffel abgebildet?','Q1_G3_F1_B1_klein.png','Q1_G3_F1_B1.png','Q1_G3_F1_B2_klein.png','Q1_G3_F1_B2.png','Q1_G3_F1_B3_klein.png','Q1_G3_F1_B3.png','Q1_G3_F1_B4_klein.png','Q1_G3_F1_B4.png',1,0,1,0);
INSERT INTO "QuestionsImageChecker" VALUES (6,3,'Crop','Wo sehen Sie die Feldfruchtsorte Gerste abgebildet?','Q1_G3_F2_B1_klein.png','Q1_G3_F2_B1.png','Q1_G3_F2_B2_klein.png','Q1_G3_F2_B2.png','Q1_G3_F2_B3_klein.png','Q1_G3_F2_B3.png','Q1_G3_F2_B4_klein.png','Q1_G3_F2_B4.png',0,0,1,0);
INSERT INTO "AllQuestion" VALUES (1,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (2,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (3,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (4,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (5,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (6,'ImageCheckerPage');
COMMIT;
