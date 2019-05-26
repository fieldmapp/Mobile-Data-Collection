BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "QuestionsStadium" (
	"QuestionId"	INTEGER,
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
	"QuestionId"	INTEGER,
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
	"QuestionId"	INTEGER,
	"Skill"	TEXT,
	"Text"	TEXT,
	PRIMARY KEY("QuestionId")
);
CREATE TABLE IF NOT EXISTS "QuestionsImageChecker" (
	"QuestionId"	INTEGER,
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
	"QuestionId"	INTEGER,
	"QuestionType"	TEXT,
	PRIMARY KEY("QuestionId")
);
INSERT INTO "QuestionsImageChecker" VALUES (1,1,'Crop','Wo sehen Sie die Feldfruchtsorte Weizen abgebildet?','Q1_G1_F1_B1_klein','Q1_G1_F1_B1','Q1_G1_F1_B2_klein','Q1_G1_F1_B2','Q1_G1_F1_B3_klein','Q1_G1_F1_B3','Q1_G1_F1_B4_klein','Q1_G1_F1_B4',NULL,NULL,NULL,NULL);
INSERT INTO "QuestionsImageChecker" VALUES (2,1,'Crop','Wo sehen Sie die Feldfruchtsorte Raps abgebildet?','Q1_G1_F2_B1_klein','Q1_G1_F2_B1','Q1_G1_F2_B2_klein','Q1_G1_F2_B2','Q1_G1_F2_B3_klein','Q1_G1_F2_B3','Q1_G1_F2_B4_klein','Q1_G1_F2_B4',NULL,NULL,NULL,NULL);
INSERT INTO "QuestionsImageChecker" VALUES (3,2,'Crop','Wo sehen Sie die Feldfruchtsorte Raps abgebildet?','Q1_G2_F1_B1_klein','Q1_G2_F1_B1','Q1_G2_F1_B2_klein','Q1_G2_F1_B2','Q1_G2_F1_B3_klein','Q1_G2_F1_B3','Q1_G2_F1_B4_klein','Q1_G2_F1_B4',NULL,NULL,NULL,NULL);
INSERT INTO "QuestionsImageChecker" VALUES (4,2,'Crop','Wo sehen Sie die Feldfruchtsorte Weizen abgebildet?','Q1_G2_F2_B1_klein','Q1_G2_F2_B1','Q1_G2_F2_B2_klein','Q1_G2_F2_B2','Q1_G2_F2_B3_klein','Q1_G2_F2_B3','Q1_G2_F2_B4_klein','Q1_G2_F2_B4',NULL,NULL,NULL,NULL);
INSERT INTO "QuestionsImageChecker" VALUES (5,3,'Crop','Wo sehen Sie die Feldfruchtsorte Kartoffel abgebildet?','Q1_G3_F1_B1_klein','Q1_G3_F1_B1','Q1_G3_F1_B2_klein','Q1_G3_F1_B2','Q1_G3_F1_B3_klein','Q1_G3_F1_B3','Q1_G3_F1_B4_klein','Q1_G3_F1_B4',NULL,NULL,NULL,NULL);
INSERT INTO "QuestionsImageChecker" VALUES (6,3,'Crop','Wo sehen Sie die Feldfruchtsorte Gerste abgebildet?','Q1_G3_F2_B1_klein','Q1_G3_F2_B1','Q1_G3_F2_B2_klein','Q1_G3_F2_B2','Q1_G3_F2_B3_klein','Q1_G3_F2_B3','Q1_G3_F2_B4_klein','Q1_G3_F2_B4',NULL,NULL,NULL,NULL);
INSERT INTO "AllQuestion" VALUES (1,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (2,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (3,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (4,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (5,'ImageCheckerPage');
INSERT INTO "AllQuestion" VALUES (6,'ImageCheckerPage');
COMMIT;
