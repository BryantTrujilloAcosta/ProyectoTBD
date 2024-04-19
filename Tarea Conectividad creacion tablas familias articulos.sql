create database ventas
go
use ventas
go
create table familias(
famid int not null,
famnombre varchar( 20) not null )
go
alter table familias add constraint pk_familias primary key ( famid)
go
create table articulos(
artid int not null,
artnombre varchar( 50 ) not null, 
artdescripcion varchar( 500) not null,
artprecio numeric( 12,2) not null,
famid int not null
)
go
alter table articulos add 
constraint pk_articulos primary key ( artid),
constraint fk_articulos_familias foreign key ( famid ) references familias(famid) 
go
insert familias values( 1, 'Abarrotes' ) 
insert familias values( 2, 'Verduras' ) 
insert familias values( 3, 'Lacteos' ) 
insert familias values( 4, 'Limpieza' ) 
go
insert articulos values( 1, 'Sal la fina', 'Sal de mar',12.34  , 1) 
insert articulos values( 2, 'Cajeta Coronado', 'Cajeta de cabra', 34.34 , 1 ) 
insert articulos values( 3, 'Limón', 'Limón colima',3.45 ,2 ) 
insert articulos values( 4, 'Tomate', 'Tomate bola',21.12 ,2 ) 
insert articulos values( 5, 'Queso crema', 'Queso de vaca ligth', 43.45 , 3 ) 
insert articulos values( 6, 'Salchicha', 'Salchicha alemana',56.34  , 3) 
insert articulos values( 7, 'Trapeador', 'Trapeador rojo de madera',78.54 ,4 ) 
insert articulos values( 8, 'Cloro', 'Cloro con aroma floral', 89.87, 4) 

-- Crear usuarios
create login parca with password = '123'
CREATE LOGIN bryant WITH PASSWORD = '123';
CREATE LOGIN brayan WITH PASSWORD = '123'
CREATE LOGIN francisco WITH PASSWORD = '123'
-- Asignar roles a los usuarios
create login manuel with password = '123';
USE ventas
Create user parca for login parca
CREATE USER bryant FOR LOGIN bryant
CREATE USER brayan FOR LOGIN brayan
CREATE USER francisco FOR LOGIN francisco


-- Asignar permisos sobre las tablas
GRANT SELECT, INSERT, UPDATE, DELETE ON familias TO bryant
GRANT SELECT, INSERT, UPDATE, DELETE ON articulos TO bryant

GRANT SELECT, INSERT, UPDATE, DELETE ON familias TO brayan
GRANT SELECT, INSERT, UPDATE, DELETE ON articulos TO brayan

GRANT SELECT, INSERT, UPDATE, DELETE ON familias TO francisco
GRANT SELECT, INSERT, UPDATE, DELETE ON articulos TO francisco

-- Asignar permisos sobre procedimientos almacenados (sp)
GRANT EXECUTE TO bryant
GRANT EXECUTE TO brayan
GRANT EXECUTE TO francisco
go
-- procedimiento alamcenado
CREATE PROCEDURE sp_captura
    @artid INT OUTPUT,
    @artnombre VARCHAR(50),
    @artdescripcion VARCHAR(500),
    @artprecio NUMERIC(12, 2),
    @famid INT
AS
BEGIN
    SET NOCOUNT ON

    -- comprobar si ya existe
    IF EXISTS (SELECT * FROM articulos WHERE artid = @artid)
    BEGIN
        -- actualizar registro
        UPDATE articulos
        SET
            artnombre = @artnombre,
            artdescripcion = @artdescripcion,
            artprecio = @artprecio,
            famid = @famid
        WHERE
            artid = @artid;
		if @@ERROR<>0
        
		BEGIN 
	       RAISERROR ('Error al actualizar en la tabla articulos',16,10)
	    
		END
    END
        ELSE
           BEGIN
		      SELECT @artid = COALESCE(MAX(artid),0)+1
			  from articulos
			     insert articulos(artid,artnombre,artdescripcion,artprecio,famid)
				 values (@artid,@artnombre,@artdescripcion,@artprecio,@famid)
		       
			   if @@error <>0
			    
				BEGIN
				   RAISERROR('ERROR AL INSERTAR EN LA TABLA ARTICULOS',16,10)
                END
            END
	    END
go
CREATE PROCEDURE sp_modificar_articulo
    @artid INT,
    @artnombre VARCHAR(50),
    @artdescripcion VARCHAR(500),
    @artprecio NUMERIC(12, 2),
    @famid INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE articulos
    SET
        artnombre = @artnombre,
        artdescripcion = @artdescripcion,
        artprecio = @artprecio,
        famid = @famid
    WHERE
        artid = @artid;
END
go
CREATE PROCEDURE sp_eliminar_articulo
    @artid INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM articulos
    WHERE
        artid = @artid;
END
go
CREATE TRIGGER TR_ARTICULOS
ON ARTICULOS
FOR INSERT 
AS BEGIN
   RAISERROR ('No se permiten inserciones en la tabla ventas',16,1)
   ROLLBACK
END 
go
DROP TRIGGER TR_ARTICULOS
GO

CREATE LOGIN NATAMONTANA WITH PASSWORD ='123'
USE VENTAS
CREATE USER NATAMONTANA

GRANT SELECT, INSERT ON ARTICULOS TO NATAMONTANA
GRANT SELECT, INSERT ON FAMILIAS TO NATAMONTANA

GRANT EXECUTE TO NATAMONTANA