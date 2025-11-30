# Main Story Flow & Tutorial Sequence

## 1. Tutorial - Movement

1. **Task Start**: Scene transitions to the entrance of classroom 102, camera facing inside the room.
2. **UI**: Movement tutorial prompt.
3. **Cat**: None.
4. **End Condition**: Player reaches their seat.

## 2. Tutorial - Item Interaction

1. **Task Start**: Automatically triggered after previous step.
2. **UI**: Item interaction tutorial prompt.
3. **Cat**: None.
4. **End Condition**: Player picks up an object and puts it into the inventory.

## 3. Cutscene - Dozing Off

1. **Task Start**: "Take a nap" button appears on the desk.
2. **End Condition**: Player presses the "Take a nap" button.
3. **Task End**: Screen fades to complete black.

## 4. Air Raid Siren

1. **Task Start**: After 5 seconds → play air raid siren, trigger NPC panic & escape animations, force camera toward door then release control, spawn cat companion, show safe zones.
2. **UI**: Choice prompt - "Hide" or "Run out of the classroom".
3. **Cat**:
   - Dialogue:
     - Before leaving: "This is an air raid siren! Quickly hide here!!"
     - Before leaving (repeat): "This is an air raid siren! Quickly hide here!!"
     - When hiding: "Get down and stay here!"
   - Sets Guide Target to nearest safe zone.
4. **End Conditions** (any one triggers end):
   - Player leaves the classroom.
   - Player enters a safe zone and presses "Lie Down".
   - Random delay of 1–2 minutes.
5. **Task End**: Bombing begins.

## 5. Waiting for Bombing to End (Phase 1)

1. **Task Start**: Check player position → if not in safe zone → injury or death judgment.
2. **UI**: "Waiting for bombing to end..."
3. **Cat**: Stays in place.
4. **End Condition**: Bombing sequence finishes.
5. **Task End**: Trigger collapsing walls/glass, disable safe zone indicators.

## 6. Leave Classroom → Underground Parking (Exit 1)

1. **Task Start**: Spawn/activate cat, restore player vision.
2. **UI**: "Head to Exit 1".
3. **Cat**:
   - Sets Guide Target to area in front of Exit 1.
   - Dialogue:
     - Before leaving: "The classroom structure is too fragile and unsafe. We need to get to the underground parking."
     - While navigating: "Watch out for nearby glass; another wave could come at any moment."
     - On arrival: "This area is covered in shattered glass. Let's try a safer entrance."
4. **End Condition**: Reach Exit 1.

## 7. Underground Parking (Exit 2)

1. **Task Start**: Activate cat.
2. **UI**: "Head to Exit 2".
3. **Cat**:
   - Sets Guide Target to area in front of Exit 2.
   - Dialogue:
     - Before leaving: "Careful of the glass. Let's check another spot."
     - While navigating: "Stay close to walls and watch your surroundings."
     - On arrival: "This one is blocked by rubble. Let's try another."
4. **End Condition**: Reach Exit 2.

## 8. Underground Parking (Exit 3)

1. **Task Start**: Activate cat.
2. **UI**: "Head to Exit 3".
3. **Cat**:
   - Sets Guide Target to area in front of Exit 3.
   - Dialogue:
     - Before leaving: "Avoid open areas. Let's check another location."
     - While navigating: "Stick to walls and watch for fragile objects."
     - On arrival: "This one is blocked too. Let's find another."
4. **End Condition**: Reach Exit 3.

## 9. Underground Parking (Exit 4) - Drone Appears

1. **Task Start**: Spawn drone, activate cat.
2. **UI**: "Head to Exit 4".
3. **Cat**:
   - Sets Guide Target to area in front of Exit 4.
   - Dialogue:
     - Before leaving: "I hear a drone. Stay low and avoid it."
     - While navigating: "If you hear the drone, hide immediately."
     - On arrival: "Looks clear here. This is our way out — but check for cover first."
4. **End Condition**: Reach Exit 4 and open the door.

## 10. Prepare to Leave Building

1. **Task Start**: Automatic.
2. **UI**: "Exit the building and circle around to the parking garage from outside".
3. **Cat**: None.
4. **End Condition**: Player steps outside the door.

## 11. Outdoor Second Bombing Wave

1. **Task Start**: Shortly after → play incoming artillery sound, show safe zones, activate cat.
2. **UI**: "Avoid the second bombing wave".
3. **Cat**:
   - Sets Guide Target to nearest safe spot.
   - Dialogue: "Quick, hide here!"
4. **End Condition**: Bombing begins.

## 12. Waiting for Bombing to End (Phase 2)

1. **Task Start**: Check player position → if not in safe zone → injury or death judgment.
2. **UI**: "Waiting for bombing to end..."
3. **Cat**: Stays in place.
4. **End Condition**: Bombing sequence finishes.
5. **Task End**: Trigger collapsing walls/glass, disable safe zone indicators.

## 13. Head to Underground Parking - Outdoor Route 1

1. **Task Start**: Activate cat, restore player vision.
2. **UI**: "Circle around outside to reach the parking garage".
3. **Cat**:
   - Sets Guide Target to the crossroads.
   - Dialogue:
     - Before leaving: "We survived that one. Observe the next section carefully before moving."
     - While navigating: "Stay close to walls, avoid glass and trees."
4. **End Condition**: Pass through the crossroads.

## 14. Head to Underground Parking - Outdoor Route 2 (Final Stretch)

1. **Task Start**: Spawn drone, activate cat.
2. **UI**: "Reach the parking garage".
3. **Cat**:
   - Sets Guide Target to parking garage entrance.
   - Dialogue:
     - Before leaving: "The last stretch has no cover — move quickly."
     - While navigating: "Don't attract the drone's attention. They usually won't attack civilians. Keep moving — the garage is close!"
4. **End Condition**: Enter the parking garage.
5. **Task End**: Trigger ending/settlement screen.
