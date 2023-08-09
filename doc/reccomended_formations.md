# Reccomended Formations

A non-exauhstive list of "Reccomended Formations" are stored in `/ProjectDatas/ReccomendedForm` as ".rf" files. They are of the format of a JSON array. They appear to be paginated as an optimization, and for no other reason.

`/ProjectDatas/ReccomendedForm` is downloaded when you sign in if it does not exist, has been corrupted, heavily modified, etc.

Sometimes modifications persist, sometimes they don't.

`"direction": 0`` - horizontal
`"direction": 0`` - vertical

Changing the direction of everything doesn't work; Chagning the name does.

Reccomendations are programatically created when high MMR players deploy with the same formation many times.

The number of wins and the number of times users deploy with the reccomendation is tracked and that informs how they are shown to the player

Basically, the three formations which are most likely to result in a win out of all formations that match your exact unit composition are shown

I think X is left and right, and Y is forwards/backwards.

The game can take reccomendations from the internet that don't exist on file. I think the local ones are appended to ones downloaded from the internet, or this is a caching optimization.

In code, the list of reccomendations is `GameRiver.Client.RecommendFormManager.dataInfo`
