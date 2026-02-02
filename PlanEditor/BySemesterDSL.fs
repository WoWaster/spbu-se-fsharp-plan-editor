module PlanEditor.BySemesterDSL

open BySemesterBuilders
open BySemesterModel

let fgosCodeDisciplines = Disciplines
let fgosCodePracticalTraining = PracticalTraining
let fgosCodeGia = Gia

let exam = Exam
let credit = Credit
let attestationTest = AttestationTest

let bachelor = Bachelor
let master = Master

let russian = Russian
let english = English

let classroomWork = ClassroomWorkBuilder()
let independentWork = IndependentWorkBuilder()
let discipline = DisciplineBuilder()
let simpleBlock = SimpleBlockBuilder()
let complexBlock = ComplexBlockBuilder()
let blocks = BlocksBuilder()
let semester = SemesterBuilder()
let plan = PlanBuilder()
let simpleDiscipline = SimpleDisciplineBuilder()
