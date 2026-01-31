open PlanEditor.BySemesterModel


let competencies =
    [ "УК-1",
      "Способен осуществлять поиск, критический анализ и синтез информации, применять системный подход для решения поставленных задач"
      "УК-3", "Способен осуществлять социальное взаимодействие и реализовывать свою роль в команде"
      "УК-8",
      "Способен создавать и поддерживать в повседневной жизни и в профессиональной деятельности безопасные условия жизнедеятельности для сохранения природной среды, обеспечения устойчивого развития общества, в том числе при угрозе и возникновении чрезвычайных ситуаций и военных конфликтов"
      "ОПК-1",
      "Способен применять фундаментальные знания, полученные в области математических и (или) естественных наук, и использовать их в профессиональной деятельности"
      "ОПК-2",
      "Способен применять современный математический аппарат, связанный с проектированием, разработкой, реализацией и оценкой качества программных продуктов и программных комплексов в различных областях человеческой деятельности"
      "ОПК-3",
      "Способен понимать и применять современные информационные технологии, в том числе отечественные, при создании программных продуктов и программных комплексов различного назначения"
      "ОПК-4",
      "Способен участвовать в разработке технической документации программных продуктов и программных комплексов"
      "ПКА-1",
      "Способен демонстрировать базовые знания математических и естественных наук, программирования и информационных технологий"
      "ПКП-10-А-ПК-1", "Осуществляет управление архитектурой изолированной (неинтегрированной) программной системы"
      "ПКП-13-А-ПК-4", "Осуществляет оценки и управление рисками" ]


let bzhd =
    { FgosBlockCode = Disciplines
      Workload = 3
      Competencies = [ "УК-8" ]
      Disciplines =
        [ { Number = 073519
            Name = "Безопасность жизнедеятельности"
            EnglishName = "Life Safety"
            Realization = ""
            Trajectory = ""
            AssessmentForms = [ Credit ]
            ClassroomWork =
              { defaultClassroomWork with
                  Lectures = 26
                  PracticalClasses = 34
                  IntermediateAssessment = 4 }
            IndependentWork =
              { defaultIndependentWork with
                  InInstructorPresence = 8
                  UsingMaterials = 34
                  IntermediateAssessment = 2 }
            InteractiveHours = 0 } ] }

let practicalTrainingStandard =
    { FgosBlockCode = PracticalTraining
      Workload = 3
      Competencies =
        [ "ОПК-1"
          "ОПК-2"
          "ОПК-3"
          "ОПК-4"
          "ПКА-1"
          "ПКП-10-А-ПК-1"
          "ПКП-13-А-ПК-4"
          "УК-1"
          "УК-3" ]
      Disciplines =
        [ { Number = 064793
            Name = "Учебная практика 2 (научно-исследовательская работа)"
            EnglishName = "Practical Training 2 (Research Project)"
            Realization = ""
            Trajectory = ""
            AssessmentForms = [ Credit ]
            ClassroomWork =
              { defaultClassroomWork with
                  IntermediateAssessment = 2 }
            IndependentWork =
              { defaultIndependentWork with
                  InInstructorPresence = 30
                  UsingMaterials = 68
                  IntermediateAssessment = 8 }
            InteractiveHours = 8 } ] }

let teorverTop =
    { FgosBlockCode = Disciplines
      Workload = 2
      Competencies = [ "ОПК-1"; "ПКА-1" ]
      Disciplines =
        [ { Number = 002188
            Name = "Теория вероятностей и математическая статистика"
            EnglishName = "Probability Theory and Mathematical Statistics"
            Realization = "осн курс"
            Trajectory = "тр 3 г"
            AssessmentForms = [ Credit ]
            ClassroomWork =
              { defaultClassroomWork with
                  Lectures = 30
                  PracticalClasses = 12
                  ControlWorks = 2
                  IntermediateAssessment = 2 }
            IndependentWork =
              { defaultIndependentWork with
                  UsingMaterials = 18
                  IntermediateAssessment = 8 }
            InteractiveHours = 12 } ] }

let semester5 =
    { Number = 5
      BasicBlocks =
        { SimpleBlocks = [ bzhd ]
          ComplexBlocks =
            [ { Name = ":"
                Tracks =
                  [ "Технологии программирования  — \"общий профиль\"", [ practicalTrainingStandard ]
                    "Технологии программирования — \"профиль ТОП ИТ\"", [ teorverTop ] ]
                  |> Map.ofList } ] }
      ElectiveBlocks =
        { SimpleBlocks = []
          ComplexBlocks = [] } }

let up =
    { Name = "Технологии программирования"
      EnglishName = "Technology Programming"
      StudyLevel = Bachelor
      Specialty = "02.03.03 Математическое обеспечение и администрирование информационных систем"
      LanguagesOfInstruction = [ Russian; English ]
      YearOfAdmission = 25
      Code = 5162
      Competencies = Map.ofList competencies
      Semesters = [ semester5 ] }

printfn "%A" up
