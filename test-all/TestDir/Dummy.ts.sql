/*
	SQL文サンプル
*/
select *
from Dummy
where 1 = 1
and   Id = @Id

/* IF @Name != '' */
and   Name = @Name
/* ELSE IF @Age >= 20 */
and   Age = @Age
/* END */

order by
	Id,
	Name
