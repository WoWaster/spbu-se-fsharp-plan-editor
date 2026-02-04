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
let simpleDiscipline = SimpleDisciplineBuilder()
let disciplineInBlock = DisciplineInBlockBuilder()
let electiveBlock = ElectiveBlockBuilder()
// let complexBlock = ComplexBlockBuilder()
let basicPart = BasicPartBuilder()
let variablePart = VariablePartBuilder()
let complexBlock = ComplexBlockBuilder()
let semester = SemesterBuilder()
let plan = PlanBuilder()
