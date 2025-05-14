/*
	SQL文サンプル
*/
select *
from Dummy
where 1 = 1

/* IF @Name != '' */
and   Name = @Name
/* END */

/* IF @Age >= 20 */
and   Age = @Age
/* END */

order by
	Id,
	Name
