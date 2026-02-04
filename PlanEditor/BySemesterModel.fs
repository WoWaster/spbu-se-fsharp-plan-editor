module PlanEditor.BySemesterModel

// Модель учебного плана в ИС СПбГУ в упрощенном случае выглядит примерно так.
//
// Существует сама сущность учебного плана с некоторой метаинформацией, в неё вложены семестры.
// Каждый семестр состоит из трех частей:
// 1. Базовая часть.
// 2. Вариативная часть.
// 3. Факультативная часть.
//
// Стоит отметить, что Вариативная часть --- наследие старых ФГОС, и расположение предметов в базовой
// или вариативной части --- методический, а не формальный вопрос.
// Например английский в текущем исполнении обычно оказывается в Базовой части, хотя формально
// должен быть в Вариативной.
//
// Каждая из частей может содержать один из двух видов блоков:
// 1. Простой блок.
// 2. Сложный блок.
//
// Простой блок --- типичный способ выразить дисциплину по выбору, в частном случае --- единичный предмет.
// Сложный блок (он же блок дисциплин по выбору) позволяет выстроить трек сквозь несколько семестров,
// в таком случае требуется и название набору треков, и каждому конкретному треку.
// При этом внутри сложного блока естественным образом содержатся простые блоки.

type FgosBlockCode =
    | Disciplines
    | PracticalTraining
    | Gia

type AssessmentForm =
    | Exam
    | Credit
    | AttestationTest

type ClassroomWork =
    { Lectures: int
      Seminars: int
      Consultations: int
      PracticalClasses: int
      LaboratoryWorks: int
      ControlWorks: int
      Colloquiums: int
      CurrentAssessment: int
      IntermediateAssessment: int }

    static member Empty =
        { Lectures = 0
          Seminars = 0
          Consultations = 0
          PracticalClasses = 0
          LaboratoryWorks = 0
          ControlWorks = 0
          Colloquiums = 0
          CurrentAssessment = 0
          IntermediateAssessment = 0 }

type IndependentWork =
    { UnderInstructorSupervision: int
      InInstructorPresence: int
      UsingMaterials: int
      CurrentAssessment: int
      IntermediateAssessment: int }

    static member Empty =
        { UnderInstructorSupervision = 0
          InInstructorPresence = 0
          UsingMaterials = 0
          CurrentAssessment = 0
          IntermediateAssessment = 0 }

// Пояснение про набор полей Name, EnglishName, Realization, Trajectory.
// С первыми двумя казалось бы понятно, но у нас у дисциплин бывает "Реализации" и "Траектория".
// Отражаются они только в русском названии.
// При там маска примерно такая "<Название> (<Реализация>), <Траектория>".
// При том, кажется, Реализация бывает без траектории (онлайн-курсы), но Траектория только с Реализацией.
// TODO: Спросить Фролову???
//
// С точки зрения типов хорошо бы сделать их option, но поскольку пустая строка в каком-то смысле None,
// то пусть будет, тем более, что null у нас без отдельных флагов здесь нет.
type Discipline =
    { Number: int
      Name: string
      EnglishName: string
      Realization: string
      Trajectory: string
      AssessmentForms: AssessmentForm list
      ClassroomWork: ClassroomWork
      IndependentWork: IndependentWork
      InteractiveHours: int }

    static member Empty =
        { Number = 0
          Name = ""
          EnglishName = ""
          Realization = ""
          Trajectory = ""
          AssessmentForms = []
          ClassroomWork = ClassroomWork.Empty
          IndependentWork = IndependentWork.Empty
          InteractiveHours = 0 }

type SimpleBlock =
    { FgosBlockCode: FgosBlockCode
      Workload: int
      Competencies: string list
      Disciplines: Discipline list }

    static member Empty =
        { FgosBlockCode = Disciplines
          Workload = 0
          Competencies = []
          Disciplines = [] }

type ComplexBlock =
    { Name: string
      Tracks: Map<string, SimpleBlock list> }

    static member Empty = { Name = ""; Tracks = Map.empty }

type Blocks =
    { SimpleBlocks: SimpleBlock list
      ComplexBlocks: ComplexBlock list }

    static member Empty =
        { SimpleBlocks = []
          ComplexBlocks = [] }

// Currently may have been avoid, but for simplicity of DSL
// this seemed to be most simple solution
type BasicPart =
    | BasicPart of Blocks

    static member Empty = BasicPart Blocks.Empty

    member this.SimpleBlocks =
        match this with
        | BasicPart bp -> bp.SimpleBlocks

    member this.ComplexBlocks =
        match this with
        | BasicPart bp -> bp.ComplexBlocks

type VariablePart =
    | VariablePart of Blocks

    static member Empty = VariablePart Blocks.Empty

    member this.SimpleBlocks =
        match this with
        | VariablePart vp -> vp.SimpleBlocks

    member this.ComplexBlocks =
        match this with
        | VariablePart vp -> vp.ComplexBlocks


// TODO: Факультативы
type Semester =
    { Number: int
      BasicPart: BasicPart
      VariablePart: VariablePart }

    static member Empty =
        { Number = 0
          BasicPart = BasicPart.Empty
          VariablePart = VariablePart.Empty }

// TODO: Специалитет?
type StudyLevel =
    | Bachelor
    | Master

type LanguageOfInstruction =
    | Russian
    | English

// Здесь должна быть ещё форма, но у нас бывает только очная,
// а также срок обучения, но у нас он только 4 или 2 года пока что
type Plan =
    { Name: string
      EnglishName: string
      StudyLevel: StudyLevel
      Specialty: string
      LanguagesOfInstruction: LanguageOfInstruction list
      YearOfAdmission: int
      Code: int
      Competencies: Map<string, string>
      Semesters: Semester list }

    static member Empty =
        { Plan.Name = ""
          EnglishName = ""
          StudyLevel = Bachelor
          Specialty = ""
          LanguagesOfInstruction = [ Russian; English ]
          YearOfAdmission = 0
          Code = 0
          Competencies = Map.empty
          Semesters = [] }
