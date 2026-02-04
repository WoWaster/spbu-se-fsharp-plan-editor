open PlanEditor.ExcelExport
open PlanEditor.BySemesterDSL

let upCompetencies =
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
    |> Map.ofList

let cw =
    classroomWork {
        lectures 26
        practicalClasses 34
        intermediateAssessment 4
    }

let iw =
    independentWork {
        inInstructorPresence 8
        usingMaterials 34
        intermediateAssessment 2
    }

let bzhd =
    simpleDiscipline {
        workload 3
        competencies [ "УК-8" ]
        number 73519
        name "Безопасность жизнедеятельности"
        englishName "Life Safety"
        assessmentForms [ credit ]
        cw
        iw
    }


let up =
    plan {
        name "Технологии программирования"
        englishName "Technology Programming"
        specialty "02.03.03 Математическое обеспечение и администрирование информационных систем"
        yearOfAdmission 25
        code 5162
        competencies upCompetencies

        semesters
            [ semester {
                  number 5

                  basicPart {
                      bzhd

                      complexBlock {
                          name ":"

                          track
                              "Технологии программирования  — \"общий профиль\""
                              [ simpleDiscipline {
                                    fgosBlockCode fgosCodePracticalTraining
                                    workload 3

                                    competencies
                                        [ "ОПК-1"
                                          "ОПК-2"
                                          "ОПК-3"
                                          "ОПК-4"
                                          "ПКА-1"
                                          "ПКП-10-А-ПК-1"
                                          "ПКП-13-А-ПК-4"
                                          "УК-1"
                                          "УК-3" ]

                                    number 064793
                                    name "Учебная практика 2 (научно-исследовательская работа)"
                                    englishName "Practical Training 2 (Research Project)"
                                    assessmentForms [ credit ]
                                    classroomWork { intermediateAssessment 2 }

                                    independentWork {
                                        inInstructorPresence 30
                                        usingMaterials 68
                                        intermediateAssessment 8
                                    }

                                    interactiveHours 8
                                } ]

                          track
                              "Технологии программирования — \"профиль ТОП ИТ\""
                              [ simpleDiscipline {
                                    workload 2
                                    competencies [ "ОПК-1"; "ПКА-1" ]
                                    number 002188
                                    name "Теория вероятностей и математическая статистика"
                                    englishName "Probability Theory and Mathematical Statistics"
                                    realization "осн курс"
                                    trajectory "тр 3 г"
                                    assessmentForms [ credit ]

                                    classroomWork {
                                        lectures 30
                                        practicalClasses 12
                                        controlWorks 2
                                        intermediateAssessment 2
                                    }

                                    independentWork {
                                        usingMaterials 18
                                        intermediateAssessment 8
                                    }

                                    interactiveHours 12
                                }

                                ]
                      }
                  }
              }
              semester {
                  number 8

                  variablePart {
                      complexBlock {
                          name ":"

                          track
                              "Технологии программирования — \"профиль ТОП ИТ\""
                              [ electiveBlock {
                                    workload 2
                                    competencies [ "ПКП-1-ИП-ПК-1" ]

                                    disciplines
                                        [ disciplineInBlock {
                                              number 81474
                                              name "Программирование на F#"
                                              englishName "F# Programming"
                                              assessmentForms [ credit ]

                                              classroomWork {
                                                  practicalClasses 20
                                                  intermediateAssessment 2
                                              }

                                              independentWork {
                                                  usingMaterials 42
                                                  intermediateAssessment 8
                                              }

                                              interactiveHours 20
                                          }
                                          disciplineInBlock {
                                              number 2299
                                              name "Функциональное программирование"
                                              englishName "Functional Programming"
                                              realization "осн курс"
                                              trajectory "тр 8 сем"
                                              assessmentForms [ credit ]

                                              classroomWork {
                                                  seminars 20
                                                  intermediateAssessment 2
                                              }

                                              independentWork {
                                                  usingMaterials 42
                                                  intermediateAssessment 8
                                              }

                                              interactiveHours 4
                                          } ]
                                } ]
                      }
                  }
              } ]
    }

printfn "%A" up
up |> exportToExcel @"out.xlsx"
