module PlanEditor.Types

// Тип дисциплины
type CourseType =
    | Base
    | Elective
    | Facultative

// Компетенции
type Competence = string

// Запись о месте в блоке выбора для элективов
type ElectivesBlockEntry = {
  Semester: int
  Number: int
  Specialization: string
}

// Распределение часов (15 значений из строки WorkHours)
type WorkHoursDistribution ={
      Lecture: int
      Seminar: int
      Consultation: int
      Practical: int
      Lab: int
      ControlWorks: int
      Colloquium: int
      CurrentControl: int
      InterimAssessment: int
      WithTeacherPresence: int
      WithTeacher: int
      WithMethodologicalMaterials: int
      CurrentControlIndependent: int
      MidtermAssessment: int
      TotalIndependentWork: int
    }

// type MonitoringType =
//     | Экзамен
//     | Зачет
//     | АттестационноеИспытание
//     | ТекущийКонтроль


type FgosBlockCode =
    | Disciplines
    | PracticalTraining
    | Gia

type Implementation = { 
    Semester: int
    LaborIntensity: int
    [<System.Text.Json.Serialization.JsonIgnore>]
    BlockCode: FgosBlockCode
    Realization: string
    Trajectory: string
    MonitoringTypes: string
    WorkHours: WorkHoursDistribution
    Competences: Competence list
}

type Course = {
      Code: string
      RussianName: string
      EnglishName: string
      Type: CourseType
      ElectivesBlock: ElectivesBlockEntry list
      Implementations: Implementation list
    }

let emptyWorkHours = {
    Lecture = 0
    Seminar = 0
    Consultation = 0
    Practical = 0
    Lab = 0
    ControlWorks = 0
    Colloquium = 0
    CurrentControl = 0
    InterimAssessment = 0
    WithTeacherPresence = 0
    WithTeacher = 0
    WithMethodologicalMaterials = 0
    CurrentControlIndependent = 0
    MidtermAssessment = 0
    TotalIndependentWork = 0
}

let emptyImplementation = {
    Semester = 0
    LaborIntensity = 0
    BlockCode = Disciplines
    Realization = ""
    Trajectory = ""
    MonitoringTypes = ""
    WorkHours = emptyWorkHours
    Competences = []
}

let emptyCourse = {
    Code = ""
    RussianName = ""
    EnglishName = ""
    Type = Base
    ElectivesBlock = []
    Implementations = []
}