/*
	SQL文サンプル
*/
select *
from Dummy
where 1 = 1
and   Id = @Id

/* IF @Name */
and   Name = @Name
/* END */

order by
	Id,
	Name
