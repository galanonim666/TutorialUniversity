CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" varchar(150) NOT NULL,
    "ProductVersion" varchar(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Instructor" (
    "ID" serial NOT NULL,
    "FirstName" varchar(50) NOT NULL,
    "HireDate" timestamp NOT NULL,
    "LastName" varchar(50) NOT NULL,
    CONSTRAINT "PK_Instructor" PRIMARY KEY ("ID")
);

CREATE TABLE "Student" (
    "ID" serial NOT NULL,
    "EnrollmentDate" timestamp NOT NULL,
    "FirstName" varchar(50) NOT NULL,
    "LastName" varchar(50) NULL,
    CONSTRAINT "PK_Student" PRIMARY KEY ("ID")
);

CREATE TABLE "Department" (
    "DepartmentID" serial NOT NULL,
    "Budget" money NOT NULL,
    "InstructorID" int4 NULL,
    "Name" varchar(50) NULL,
    "StartDate" timestamp NOT NULL,
    CONSTRAINT "PK_Department" PRIMARY KEY ("DepartmentID"),
    CONSTRAINT "FK_Department_Instructor_InstructorID" FOREIGN KEY ("InstructorID") REFERENCES "Instructor" ("ID") ON DELETE NO ACTION
);

CREATE TABLE "OfficeAssignment" (
    "InstructorID" int4 NOT NULL,
    "Location" varchar(50) NULL,
    CONSTRAINT "PK_OfficeAssignment" PRIMARY KEY ("InstructorID"),
    CONSTRAINT "FK_OfficeAssignment_Instructor_InstructorID" FOREIGN KEY ("InstructorID") REFERENCES "Instructor" ("ID") ON DELETE CASCADE
);

CREATE TABLE "Course" (
    "CourseID" int4 NOT NULL,
    "Credits" int4 NOT NULL,
    "DepartmentID" int4 NOT NULL,
    "Title" varchar(50) NULL,
    CONSTRAINT "PK_Course" PRIMARY KEY ("CourseID"),
    CONSTRAINT "FK_Course_Department_DepartmentID" FOREIGN KEY ("DepartmentID") REFERENCES "Department" ("DepartmentID") ON DELETE CASCADE
);

CREATE TABLE "CourseAssignment" (
    "CourseID" int4 NOT NULL,
    "InstructorID" int4 NOT NULL,
    CONSTRAINT "PK_CourseAssignment" PRIMARY KEY ("CourseID", "InstructorID"),
    CONSTRAINT "FK_CourseAssignment_Course_CourseID" FOREIGN KEY ("CourseID") REFERENCES "Course" ("CourseID") ON DELETE CASCADE,
    CONSTRAINT "FK_CourseAssignment_Instructor_InstructorID" FOREIGN KEY ("InstructorID") REFERENCES "Instructor" ("ID") ON DELETE CASCADE
);

CREATE TABLE "Enrollment" (
    "EnrollmentId" serial NOT NULL,
    "CourseId" int4 NOT NULL,
    "Grade" int4 NULL,
    "StudentId" int4 NOT NULL,
    CONSTRAINT "PK_Enrollment" PRIMARY KEY ("EnrollmentId"),
    CONSTRAINT "FK_Enrollment_Course_CourseId" FOREIGN KEY ("CourseId") REFERENCES "Course" ("CourseID") ON DELETE CASCADE,
    CONSTRAINT "FK_Enrollment_Student_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Student" ("ID") ON DELETE CASCADE
);

CREATE INDEX "IX_Course_DepartmentID" ON "Course" ("DepartmentID");

CREATE INDEX "IX_CourseAssignment_InstructorID" ON "CourseAssignment" ("InstructorID");

CREATE INDEX "IX_Department_InstructorID" ON "Department" ("InstructorID");

CREATE INDEX "IX_Enrollment_CourseId" ON "Enrollment" ("CourseId");

CREATE INDEX "IX_Enrollment_StudentId" ON "Enrollment" ("StudentId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20171121145613_InitDbPostgres', '2.0.0-rtm-26452');

ALTER TABLE "Department" ADD "ShadowTest" text NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20171121170526_AddShadowPropertyTest', '2.0.0-rtm-26452');

