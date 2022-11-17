Queries auxiliares para manipular la DB

//Fill databse with data
MERGE (n0:Init {name:"Init"})
MERGE (m0:Process {name:"P1"})
MERGE (n0) - [:next] - (m0)

MERGE (n1:Process {name:"P1"})
MERGE (m1:Process {name:"P2"})
MERGE (n1) - [:next] - (m1)

MERGE (n2:Process {name:"P1"})
MERGE (m2:Process {name:"P3"})
MERGE (n2) - [:next] - (m2)

MERGE (n3:Process {name:"P1"})
MERGE (m3:Process {name:"P8"})
MERGE (n3) - [:next] - (m3)

MERGE (n4:Process {name:"P2"})
MERGE (m4:Process {name:"P8"})
MERGE (n4) - [:next] - (m4)

MERGE (n5:Process {name:"P3"})
MERGE (m5:Process {name:"P4"})
MERGE (n5) - [:fork] - (m5)

MERGE (n6:Process {name:"P3"})
MERGE (m6:Process {name:"P5"})
MERGE (n6) - [:fork] - (m6)

MERGE (n7:Process {name:"P4"})
MERGE (m7:Process {name:"P6"})
MERGE (n7) - [:next] - (m7)

MERGE (n8:Process {name:"P5"})
MERGE (m8:Process {name:"P7"})
MERGE (n8) - [:join] - (m8)

 MERGE (n9:Process {name:"P6"})
 MERGE (m9:Process {name:"P7"})
 MERGE (n9) - [:join] - (m9)

 MERGE (n10:Process {name:"P7"})
 MERGE (m10:Process {name:"P8"})
 MERGE (n10) - [:next] - (m10)

 MERGE (n11:Process {name:"P8"})
 MERGE (m11:Process {name:"P9"})
 MERGE (n11) - [:fork] - (m11)

 MERGE (n12:Process {name:"P8"})
 MERGE (m12:Process {name:"P10"})
 MERGE (n12) - [:fork] - (m12)

 MERGE (n13:Process {name:"P8"})
 MERGE (m13:Process {name:"P11"})
 MERGE (n13) - [:fork] - (m13)

 MERGE (n14:Process {name:"P9"})
 MERGE (m14:Process {name:"P12"})
 MERGE (n14) - [:join] - (m14)

 MERGE (n15:Process {name:"P10"})
 MERGE (m15:Process {name:"P12"})
 MERGE (n15) - [:join] - (m15)

 MERGE (n16:Process {name:"P11"})
 MERGE (m16:Process {name:"P12"})
 MERGE (n16) - [:join] - (m16)

 MERGE (n17:Process {name:"P12"})
 MERGE (m17:Finish {name:"Finish"})
 MERGE (n17) - [:next] - (m17)
 
//--------------------------------

 MERGE (P1:Process {name:"P1"})
 MERGE (P1) - [:startsWith] - (:Activity {name:"finishWork"})
 
 MERGE (P2:Process {name:"P2"})
 MERGE (P2) - [:startsWith] - (:Activity {name:"eatOut"})
 
 MERGE (P3:Process {name:"P3"})
 MERGE (P3) - [:startsWith] - (:Activity {name:"pickUpSister"})
 
 MERGE (P4:Process {name:"P4"})
 MERGE (P4) - [:startsWith] - (:Activity {name:"eatOut"})
 
 MERGE (P5:Process {name:"P5"})
 MERGE (P5) - [:startsWith] - (:Activity {name:"spendTimeTogether"})
 
 MERGE (P6:Process {name:"P6"})
 MERGE (P6) - [:startsWith] - (:Activity {name:"takeAWalk"})
 
 MERGE (P7:Process {name:"P7"})
 MERGE (P7) - [:startsWith] - (:Activity {name:"driveSisterHome"})
 
 MERGE (P8:Process {name:"P8"})
 MERGE (P8) - [:startsWith] - (:Activity {name:"goHome"})
 
 MERGE (P9:Process {name:"P9"})
 MERGE (P9) - [:startsWith] - (:Activity {name:"takeAShower"})
 
 MERGE (P10:Process {name:"P10"})
 MERGE (P10) - [:startsWith] - (:Activity {name:"listenToMusic"})
 
 MERGE (P11:Process {name:"P11"})
 MERGE (P11) - [:startsWith] - (:Activity {name:"meditate"})
 
 MERGE (P12:Process {name:"P12"})
 MERGE (P12) - [:startsWith] - (:Activity {name:"goToBed"})

--------------------------------------------------------------
//DELETE ALL DATA
match (n) detach delete (n)

--------------------------------------------------------------
//CREATE trace for comparisons
MERGE (P:Process {name:"P1", trace:"trace1"})
MERGE (Q:Process {name:"P3", trace:"trace1"})
MERGE (P) - [:next] -> (Q)
MERGE (P) - [r:startsWith] -> (:Activity {name:"finishWork"})
MERGE (Q) - [r1:startsWith] -> (:Activity {name:"pickUpSister"})

---------------------------------------------------------------

//Query to delete all conform relations created.
MATCH (n) - [r:conformsTo] - (m)
DELETE r


TEST QUERIES 1
-----------------------------
//Query to create test data
MERGE (P1:Process:TestNode {name: "P1"}) - [:startsWith] - (:Activity:TestNode {name:"a1"})
MERGE (P2:Process:TestNode {name: "P2"}) - [:startsWith] - (:Activity:TestNode {name:"a2"})
MERGE (P3:Process:TestNode {name: "P3"}) - [:startsWith] - (:Activity:TestNode {name:"a3"})
MERGE (P4:Process:TestNode {name: "P4"}) - [:startsWith] - (:Activity:TestNode {name:"a4"})
MERGE (P5:Process:TestNode {name: "P5"}) - [:startsWith] - (:Activity:TestNode {name:"a5"})
MERGE (F1:Finish:TestNode)

MERGE (P1) - [:next] - (P2)
MERGE (P1) - [:next] - (P3)
MERGE (P1) - [:next] - (P4)
MERGE (P2) - [:next] - (P5)
MERGE (P3) - [:next] - (P5)
MERGE (P4) - [:next] - (P5)
MERGE (P5) - [:next] - (F1)

MERGE (Q1:Process:TestNode {name: "Q1"}) - [:startsWith] - (:Activity:TestNode {name:"a1"})
MERGE (Q2:Process:TestNode {name: "Q2"}) - [:startsWith] - (:Activity:TestNode {name:"a2"})
MERGE (Q3:Process:TestNode {name: "Q3"}) - [:startsWith] - (:Activity:TestNode {name:"a3"})
MERGE (Q5:Process:TestNode {name: "Q5"}) - [:startsWith] - (:Activity:TestNode {name:"a5"})
MERGE (F2:Finish:TestNode)

MERGE (Q1) - [:next] - (Q2)
MERGE (Q1) - [:next] - (Q3)
MERGE (Q2) - [:next] - (Q5)
MERGE (Q3) - [:next] - (Q5)
MERGE (Q5) - [:next] - (F2)


-----------------------------------------
//Extra traces
//Conforming 1
MERGE (n0:Init:Trace1 {name:"Init"})
MERGE (m0:Process:Trace1 {name:"P1"})
MERGE (n0) - [:next] - (m0)

MERGE (n1:Process:Trace1 {name:"P1"})
MERGE (m1:Process:Trace1 {name:"P2"})
MERGE (n1) - [:next] - (m1)

MERGE (n4:Process:Trace1 {name:"P2"})
MERGE (m4:Process:Trace1 {name:"P8"})
MERGE (n4) - [:next] - (m4)

 MERGE (n11:Process:Trace1 {name:"P8"})
 MERGE (m11:Process:Trace1 {name:"P9"})
 MERGE (n11) - [:fork] - (m11)

 MERGE (n12:Process:Trace1 {name:"P8"})
 MERGE (m12:Process:Trace1 {name:"P10"})
 MERGE (n12) - [:fork] - (m12)

 MERGE (n13:Process:Trace1 {name:"P8"})
 MERGE (m13:Process:Trace1 {name:"P11"})
 MERGE (n13) - [:fork] - (m13)

 MERGE (n14:Process:Trace1 {name:"P9"})
 MERGE (m14:Process:Trace1 {name:"P12"})
 MERGE (n14) - [:join] - (m14)

 MERGE (n15:Process:Trace1 {name:"P10"})
 MERGE (m15:Process:Trace1 {name:"P12"})
 MERGE (n15) - [:join] - (m15)

 MERGE (n16:Process:Trace1 {name:"P11"})
 MERGE (m16:Process:Trace1 {name:"P12"})
 MERGE (n16) - [:join] - (m16)

 MERGE (n17:Process:Trace1 {name:"P12"})
 MERGE (m17:Finish {name:"Finish"})
 MERGE (n17) - [:next] - (m17)
 
//--------------------------------

 MERGE (P1:Process:Trace1 {name:"P1"})
 MERGE (P1) - [:startsWith] - (:Activity {name:"finishWork"})
 
 MERGE (P2:Process:Trace1 {name:"P2"})
 MERGE (P2) - [:startsWith] - (:Activity {name:"eatOut"})
 
 MERGE (P8:Process:Trace1 {name:"P8"})
 MERGE (P8) - [:startsWith] - (:Activity {name:"goHome"})
 
 MERGE (P9:Process:Trace1 {name:"P9"})
 MERGE (P9) - [:startsWith] - (:Activity {name:"takeAShower"})
 
 MERGE (P10:Process:Trace1 {name:"P10"})
 MERGE (P10) - [:startsWith] - (:Activity {name:"listenToMusic"})
 
 MERGE (P11:Process:Trace1 {name:"P11"})
 MERGE (P11) - [:startsWith] - (:Activity {name:"meditate"})
 
 MERGE (P12:Process:Trace1 {name:"P12"})
 MERGE (P12) - [:startsWith] - (:Activity {name:"goToBed"})


 -------------------
 //Non conforming 1
 //Fill databse with data
MERGE (n0:Init:Trace2 {name:"Init"})
MERGE (m0:Process:Trace2 {name:"P1"})
MERGE (n0) - [:next] - (m0)


MERGE (n2:Process:Trace2 {name:"P1"})
MERGE (m2:Process:Trace2 {name:"P3"})
MERGE (n2) - [:next] - (m2)

MERGE (n8:Process:Trace2 {name:"P3"})
MERGE (m8:Process:Trace2 {name:"P7"})
MERGE (n8) - [:join] - (m8)

 MERGE (n10:Process:Trace2 {name:"P7"})
 MERGE (m10:Process:Trace2 {name:"P8"})
 MERGE (n10) - [:next] - (m10)

 MERGE (n11:Process:Trace2 {name:"P8"})
 MERGE (m11:Process:Trace2 {name:"P9"})
 MERGE (n11) - [:next] - (m11)

 MERGE (n14:Process:Trace2 {name:"P9"})
 MERGE (m14:Process:Trace2 {name:"P12"})
 MERGE (n14) - [:next] - (m14)

 MERGE (n15:Process:Trace2 {name:"P12"})
 MERGE (m15:Process:Trace2 {name:"P13"})
 MERGE (n15) - [:next] - (m15)

 MERGE (n17:Process:Trace2 {name:"P13"})
 MERGE (m17:Finish {name:"Finish"})
 MERGE (n17) - [:next] - (m17)
 
//--------------------------------

 MERGE (P1:Process:Trace2 {name:"P1"})
 MERGE (P1) - [:startsWith] - (:Activity {name:"finishWork"})
 
 MERGE (P3:Process:Trace2 {name:"P3"})
 MERGE (P3) - [:startsWith] - (:Activity {name:"pickUpSister"})
 
 MERGE (P7:Process:Trace2 {name:"P7"})
 MERGE (P7) - [:startsWith] - (:Activity {name:"driveSisterHome"})
 
 MERGE (P8:Process:Trace2 {name:"P8"})
 MERGE (P8) - [:startsWith] - (:Activity {name:"goHome"})
 
 MERGE (P9:Process:Trace2 {name:"P9"})
 MERGE (P9) - [:startsWith] - (:Activity {name:"takeAShower"})
 
 MERGE (P12:Process:Trace2 {name:"P12"})
 MERGE (P12) - [:startsWith] - (:Activity {name:"watchAMovie"})

 MERGE (P13:Process:Trace2 {name:"P13"})
 MERGE (P13) - [:startsWith] - (:Activity {name:"goToBed"})



 ---------------------------------------
 rutinas con subtipos

 MERGE (P1:Process {name:"P1"})
 MERGE (P1) - [:startsWith] - (:Activity {name:"dressUp"})

 MERGE (P2:Process {name:"P2"})
 MERGE (P2) - [:startsWith] - (:Activity {name:"jogging"})

 
 MERGE (P3:Process {name:"P3"})
 MERGE (P3) - [:startsWith] - (:Activity {name:"listenToMusic"})


 MERGE (P4:Process {name:"P4"})
 MERGE (P4) - [:startsWith] - (:Activity {name:"takeAShower"})

 
 MERGE (P5:Process {name:"P5"})
 MERGE (P5) - [:startsWith] - (:Activity {name:"eatLunch"})

 MERGE (finish:Finish)
 
MERGE (P1) - [:fork] - (P2)
MERGE (P1) - [:fork] - (P3)
Merge (P2) - [:join] - (P4)
Merge (P3) - [:join] - (P4)
MERGE (P4) - [:next] - (P5)
MERGE (P5) - [:next] - (finish)

---segundo ejemplo

 MERGE (P1:Process {name:"Q1"})
 MERGE (P1) - [:startsWith] - (:Activity {name:"dressUp"})

 MERGE (P2:Process {name:"Q2"})
 MERGE (P2) - [:startsWith] - (:Activity {name:"treadmill"})

 
 MERGE (P3:Process {name:"Q3"})
 MERGE (P3) - [:startsWith] - (:Activity {name:"watchTV"})


 MERGE (P4:Process {name:"Q4"})
 MERGE (P4) - [:startsWith] - (:Activity {name:"takeABath"})

 
 MERGE (P5:Process {name:"Q5"})
 MERGE (P5) - [:startsWith] - (:Activity {name:"eatAtRestaurant"})

 MERGE (finish:Finish)
 
MERGE (P1) - [:fork] - (P2)
MERGE (P1) - [:fork] - (P3)
Merge (P2) - [:join] - (P4)
Merge (P3) - [:join] - (P4)
MERGE (P4) - [:next] - (P5)
MERGE (P5) - [:next] - (finish)

---Mirror
MERGE (P1:Process:Mirror {name:"Q1"})
MERGE (P1) - [:startsWith] - (:Activity {name:"dressUp"})

MERGE (P2:Process:Mirror {name:"Q2"})
MERGE (P2) - [:startsWith] - (:Activity {name:"treadmill"})
 
MERGE (P3:Process:Mirror {name:"Q3"})
MERGE (P3) - [:startsWith] - (:Activity {name:"watchTV"})

MERGE (P4:Process:Mirror {name:"Q4"})
MERGE (P4) - [:startsWith] - (:Activity {name:"takeABath"})
 
MERGE (P5:Process:Mirror {name:"Q5"})
MERGE (P5) - [:startsWith] - (:Activity {name:"eatAtRestaurant"})

MERGE (finish:Finish)
 
MERGE (P1) - [:fork] - (P2)
MERGE (P1) - [:fork] - (P3)
Merge (P2) - [:join] - (P4)
Merge (P3) - [:join] - (P4)
MERGE (P4) - [:next] - (P5)
MERGE (P5) - [:next] - (finish)
---

-----------------------------
Incertidumbre

MERGE (P1:Process:Uncertainty {name:"P1"})
 MERGE (P1) - [:startsWith] - (:Activity:Uncertainty {name:"dressUp"})

 MERGE (P2:Process:Uncertainty {name:"P2"})
 MERGE (P2) - [:startsWith] - (:Activity:Uncertainty {name:"jogging"})

 
 MERGE (P3:Process:Uncertainty {name:"P3"})
 MERGE (P3) - [:startsWith] - (:Activity:Uncertainty {name:"listenToMusic"})


 MERGE (P4:Process:Uncertainty {name:"P4"})
 MERGE (P4) - [:startsWith] - (:Activity:Uncertainty {name:"takeAShower"})

 
 MERGE (P5:Process:Uncertainty {name:"P5"})
 MERGE (P5) - [:startsWith] - (:Activity:Uncertainty {name:"eatLunch"})

 MERGE (finish:Finish)
 
MERGE (P1) - [:fork] - (P2)
MERGE (P1) - [:fork] - (P3)
Merge (P2) - [:join] - (P4)
Merge (P3) - [:join] - (P4)
MERGE (P4) - [:next] - (P5)
MERGE (P5) - [:next] - (finish)

---segundo ejemplo uncertainty

 MERGE (P1:Process:Uncertainty {name:"Q1"})
 MERGE (P1) - [:startsWith] - (:Activity:Uncertainty {name:"dressUp"})

 MERGE (P2:Process:Uncertainty {name:"Q2"})
 MERGE (P2) - [:startsWith] - (:Activity:Uncertainty {name:"treadmill"})

 
 MERGE (P3:Process:Uncertainty {name:"Q3"})
 MERGE (P3) - [:startsWith] - (:Activity:Uncertainty {name:"watchTV"})


 MERGE (P4:Process:Uncertainty {name:"Q4"})
 MERGE (P4) - [:startsWith] - (:Activity:Uncertainty {name:"takeABath"})

 
 MERGE (P5:Process:Uncertainty {name:"Q5"})
 MERGE (P5) - [:startsWith] - (:Activity:Uncertainty {name:"eatAtRestaurant"})

 MERGE (finish:Finish)
 
MERGE (P1) - [:fork] - (P2)
MERGE (P1) - [:fork] - (P3)
Merge (P2) - [:join] - (P4)
Merge (P3) - [:join] - (P4)
MERGE (P4) - [:next] - (P5)
MERGE (P5) - [:next] - (finish)


--- Rutina general uncertainty

 //MERGE (Init:Uncertainty {name:"Init"})
 
 MERGE (P1:Process:Uncertainty {name:"G1"})
 MERGE (P1) - [:startsWith] - (:Activity:Uncertainty {name:"dressUp"})

 MERGE (P2:Process:Uncertainty {name:"G2"})
 MERGE (P2) - [:startsWith] - (:Activity:Uncertainty {name:"exercise"})
 
 MERGE (P3:Process:Uncertainty {name:"G3"})
 MERGE (P3) - [:startsWith] - (:Activity:Uncertainty {name:"recreation"})

 MERGE (P4:Process:Uncertainty {name:"G4"})
 MERGE (P4) - [:startsWith] - (:Activity:Uncertainty {name:"takeAShower"})
 
 MERGE (P5:Process:Uncertainty {name:"G5"})
 MERGE (P5) - [:startsWith] - (:Activity:Uncertainty {name:"eatLunch"})

 MERGE (finish:Finish {name:"Finish"})
 
MERGE (Init) - [:next] - (P1)  
MERGE (P1) - [:fork] - (P2)
MERGE (P1) - [:fork] - (P3)
Merge (P2) - [:join] - (P4)
Merge (P3) - [:join] - (P4)
MERGE (P4) - [:next] - (P5)
MERGE (P5) - [:next] - (finish)



-----
para crear los subtypos
match (n:Uncertainty), (m:Uncertainty)
where n.name = "treadmill" AND m.name = "jogging"
merge (n) - [:subTypeOf{certainty:0.8}] -> (m)
return *

match (n:Uncertainty), (m:Uncertainty)
where n.name = "takeAShower" AND m.name = "takeABath"
merge (n) - [:subTypeOf{certainty:0.7}] -> (m)
return *

match (n:Uncertainty), (m:Uncertainty)
where n.name = "eatAtRestaurant" AND m.name = "eatLunch"
merge (n) - [:subTypeOf{certainty:0.8}] -> (m)
return *

match (n:Uncertainty), (m:Uncertainty)
where n.name = "treadmill" AND m.name = "exercise"
merge (n) - [:subTypeOf{certainty:1}] -> (m)
return *

match (n:Uncertainty), (m:Uncertainty)
where n.name = "jogging" AND m.name = "exercise"
merge (n) - [:subTypeOf{certainty:1}] -> (m)
return *

match (n:Uncertainty), (m:Uncertainty)
where n.name = "watchTV" AND m.name = "recreation"
merge (n) - [:subTypeOf{certainty:1}] -> (m)
return *

match (n:Uncertainty), (m:Uncertainty)
where n.name = "listenToMusic" AND m.name = "recreation"
merge (n) - [:subTypeOf{certainty:1}] -> (m)
return *




Para hacer debug añadir PROFILE al principio de la query