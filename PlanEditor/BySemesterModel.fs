module PlanEditor.BySemesterModel

type FgosBlockCode =
    | Disciplines
    | PracticalTraining
    | Gia

type AssessmentForm =
    | Exam
    | Credit
    | AttestationTest

// Эти данные вынесены в данной модели в один тип, поскольку могут быть общими
// как для одной дисциплины, так и для блока дисциплин.
type CommonInfo =
    { FgosBlockCode: FgosBlockCode
      Workload: int
      Competencies: string Set
      AssessmentForms: AssessmentForm Set }

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

let defaultClassroomWork =
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

let defaultIndependentWork =
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
      ClassroomWork: ClassroomWork
      IndependentWork: IndependentWork
      InteractiveHours: int }

type BaseDiscipline =
    { Info: CommonInfo
      Discipline: Discipline }

type BaseDisciplineBlockItem =
    { SubBlockName: string
      Disciplines: BaseDiscipline list }

type BaseDisciplineBlock =
    { BlockName: string
      Items: BaseDisciplineBlockItem list }

type ElectiveDisciplines =
    { Info: CommonInfo
      Disciplines: Discipline list }

type ElectiveDisciplinesBlockItem =
    { SubBlockName: string
      Disciplines: ElectiveDisciplines list }

type ElectiveDisciplinesBlock =
    { BlockName: string
      Items: ElectiveDisciplinesBlockItem list }

// Жутко вложенный тип, зато абсолютно безопасный с точки зрения того, что дозволенно упихивать в УП
// TODO: Поддержать факультативы
type SemesterItem =
    | BaseDiscipline of BaseDiscipline
    // На самом деле хороший вопрос как это правильно должно быть оформлено.
    // То есть довольно странно, что блоки дисциплин бывают в обязательной части.
    // Однако английский сделан именно так (а физра по-другому...)
    // А в ТП вообще в вариативной части есть блоки из одного предмета...
    // TODO: Спросить Фролову???
    | BaseDisciplineBlock of BaseDisciplineBlock
    | ElectiveDisciplines of ElectiveDisciplines
    | ElectiveDisciplinesBlock of ElectiveDisciplinesBlock

type Semester =
    { Number: int
      Disciplines: SemesterItem list }

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
      LanguagesOfInstruction: LanguageOfInstruction Set
      YearOfAdmission: int
      Code: int
      Competencies: Map<string, string>
      Semesters: Semester list }
